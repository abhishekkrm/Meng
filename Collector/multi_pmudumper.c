#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <pthread.h>
#include <string.h>
#include <netinet/in.h>
#include "c37.h"

#define DEBUG_ENABLE
#define MAX_DC_SERVER_THREADS (4)
#define BUSCONFIG_FILENAME       "busdetails.txt"
#define DFL_DC_PORT	(3361)
#define MAX_HISTORY (10)
#define DFL_DS_PORT (9999)
#define BUF_SIZE 4096
#define MAX_BUS 100

#ifdef DEBUG_ENABLE
#define DUMP_PACKET(pkt) do { \
	printf("\n-----------Packet-------\n");\
	printf("Sync: %d FrameSize: %d PacketID: %d \n",\
	(pkt->sync), (pkt->framesize), (pkt->id_code));\
	printf("SOC: %d FrameSec: %d Stat: %d \n",\
	(pkt->soc), (pkt->fracsec), (pkt->stat));\
	printf("VoltAmp: %f VoltAngle: %f \n",\
	(pkt->voltage_amplitude), (pkt->voltage_angle));\
	printf("Cur Amp: %f Cur Angle: %f \n",\
	(pkt->current_amplitude), (pkt->current_angle));\
	printf("Freq: %f CRC: %d \n",\
	(pkt->delta_frequency), (pkt->crc));\
} while(0);

#define DEBUG_MSG(msg,args...) do{ \
		printf(msg"\n",##args); \
	}while(0);
#else
#define DEBUG_MSG(msg,args...)
#define DUMP_PACKET(pkt)
#endif

/* An entry maintained per data received from DC */
typedef struct dc_data_entry {
	uint32_t 	soc; 				/* Time Stamp in c37 terminology */
	float 		voltage_amplitude; 	/* Reported to OC */
	float 		voltage_angle;		/* Reported to OC */
} dc_data_entry_t;

/* A structure passed to every data-collector-server-thread */
typedef struct data_collector_thread_data {
	int 				port;		/* Port on which this thread is listening */
	int 				thread_id;	/* Id */
	int					cur_index;	/* Current index in the below array */
	dc_data_entry_t 	received_data[MAX_HISTORY]; /* data from DC */
} data_collector_thread_data_t;

#define data_collector_thread_data_s (sizeof(data_collector_thread_data_t))

/* A structure passed to data-supplier thread */
typedef struct data_supplier_thread_data {
	int 				port;		/* Port on which this Data Supplier thread is listening */
	int                 max_dc_srv_thread; /* Max Data Collector thread */
	data_collector_thread_data_t **data_collector;
} data_supplier_thread_data_t;



/* Global stuff gleaned from program arguments.
 */
struct prog_args {
	char *name;
	char *dc_start_port;
	char *dc_instance;
	char *ds_port;
} prog_args;

static void usage(){
	fprintf(stderr, "Usage: %s [args]\n", prog_args.name);
	fprintf(stderr, "Optional argument:\n");
	fprintf(stderr, "	-c port: TCP data collector starting port [default = %d]\n", DFL_DC_PORT);
	fprintf(stderr, "	-i total instance: Number of pmudumper instance [default = %d]\n", MAX_DC_SERVER_THREADS);
	fprintf(stderr, "	-s port: TCP data supplier port [default = %d]\n", DFL_DS_PORT);
	exit(1);
}

// Make it dynamically
char *busdetails_info[100];
void init_bus_details() {
	FILE *fp = NULL;
	char *line = NULL;
	char *token = NULL;
	size_t len = 0;
	ssize_t read = 0;
	// char *token = NULL;
	int i=0;
	fp = fopen(BUSCONFIG_FILENAME, "r");
	while ((read = getline(&line, &len, fp)) != -1) {
		token = strtok(line,"\n");
		DEBUG_MSG("%s",token);
		busdetails_info[i++] = token;
	}
}
char *get_bus_details(int i){
	return busdetails_info[i];
}


static int do_copy(int fd, data_collector_thread_data_t* dc_thread_data){
	FILE *input = fdopen(fd, "r");
	if (input == 0) {
		fprintf(stderr, "%s: fd open failed\n", prog_args.name);
		exit(1);
	}
	
	/* Copy input */
	while (!feof(input)) {
		/* Read one frame */
		char buf[FRAME_SIZE];
		int n = fread(buf, FRAME_SIZE, 1, input);
		if (n == 0) {
			break;
		}
		if (n < 0) {
			perror("do_copy: fread");
			exit(1);
		}

		/* Convert the frame */
		c37_packet *pkt = get_c37_packet(buf);
		if (pkt == 0) {
			fprintf(stderr, "%s: do_copy: bad packet\n", prog_args.name);
			exit(1);
		}
		if (pkt->framesize != FRAME_SIZE) {
			fprintf(stderr, "%s: do_copy: bad frame size\n", prog_args.name);
			exit(1);
		}
		
		/* Dump packet for debugging purposes */
		//DUMP_PACKET(pkt);
		
		/* Copy data to thread buffer */
		dc_thread_data->received_data[dc_thread_data->cur_index].voltage_amplitude = pkt->voltage_amplitude;
		dc_thread_data->received_data[dc_thread_data->cur_index].voltage_angle = pkt->voltage_angle;
		dc_thread_data->received_data[dc_thread_data->cur_index].soc = pkt->soc;
		dc_thread_data->cur_index = (dc_thread_data->cur_index + 1) % MAX_HISTORY;

		/* Write the packet to standard output */
		write_c37_packet_readable(stdout, pkt);
		free(pkt);
	}

			fprintf(stderr, "%s: do_copy: close fd\n", prog_args.name);
	fclose(input);

	return 1;
}

static void do_collect(int s, data_collector_thread_data_t* dc_thread_data){
	int fd;
	for (;;) {
		if (listen(s, 1) < 0) {
			perror("listen");
			exit(1);
		}
		printf("Waiting for connection...\n");
		if ((fd = accept(s, 0, 0)) < 0) {
			perror("accept");
			exit(1);
		}

		printf("Got connection...\n");
		do_copy(fd, dc_thread_data);
		printf("Connection closed...\n");
	}
}

static void get_args(int argc, char *argv[]){
	prog_args.name = argv[0];

	int c;
	while ((c = getopt(argc, argv, "c:i:s:")) != -1) {
		switch (c) {
			case 'c':
				if (prog_args.dc_start_port != 0) {
					fprintf(stderr, "%s: can specify only one port\n",
					                prog_args.name);
					exit(1);
				}
				if ((prog_args.dc_start_port = optarg) == 0) {
					fprintf(stderr, "%s: -c takes DC start port argument\n",
					        prog_args.dc_start_port);
					exit(1);
				}
				break;
			case 'i':
				if (prog_args.dc_instance != 0) {
					fprintf(stderr, "%s: can specify number of DC instance\n",
					                prog_args.name);
					exit(1);
				}
				if ((prog_args.dc_instance = optarg) == 0) {
					fprintf(stderr, "%s: -i takes number of DC instance\n",
					        prog_args.dc_instance);
					exit(1);
				}
				break;
			case 's':
				if (prog_args.ds_port != 0) {
					fprintf(stderr, "%s: can specify only one port\n",
					                prog_args.name);
					exit(1);
				}
				if ((prog_args.ds_port = optarg) == 0) {
					fprintf(stderr, "%s: -s takes a DS port argument\n",
					        prog_args.ds_port);
					exit(1);
				}
				break;
			case '?':
			default:
				usage();
		}
	}

	/* Get the remaining args.
	 */
	if (argc - optind != 0) {
		usage();
	}
}

static void do_supply(int s, data_supplier_thread_data_t* data_supplier_thread_data)
{
	int fd, id, read_index;
	int32_t num_instances = data_supplier_thread_data->max_dc_srv_thread;
	data_collector_thread_data_t **dc_thread_data_array = data_supplier_thread_data->data_collector;

	for (; ;) {
		printf("Waiting for connection...\n");
		if ((fd = accept(s, 0, 0)) < 0) {
			perror("accept");
			exit(1);
		}
		DEBUG_MSG("Got connection from Operator console...\n");
		FILE *fp = fdopen(fd, "r");
		if (fp == 0) {
			fprintf(stderr, "%s: fp open failed\n", prog_args.name);
			exit(1);
		}
		char input[BUF_SIZE];  // MACRO SIZE
		int n = fread(input, BUF_SIZE, 1, fp);
		if (n == 0) {
			break;
		}
		if (n < 0) {
			perror("do_copy: fread");
			exit(1);
		}
		DEBUG_MSG("Input Command: %s", input);
		char output[BUF_SIZE*MAX_BUS]; // MACROSIZE* BUSNO;
		memset(&output,0,BUF_SIZE*MAX_BUS);
		//"AFFECTEDLIST":
		char end_char ='_';
		for (id = 0; id < num_instances; id++) {
			if (id == (num_instances-1)){
				end_char=':';
			}
			char busdata[BUF_SIZE];
			memset(&busdata,0,BUF_SIZE);
			/* TODO: Lock for synchronizing read and writes in data_collector_thread_data_t */
			read_index 	= dc_thread_data_array[id]->cur_index;
			sprintf(busdata,"%s,%f,%f%c",get_bus_details(id),
					dc_thread_data_array[id]->received_data[read_index].voltage_amplitude,
					dc_thread_data_array[id]->received_data[read_index].voltage_angle,end_char);
            strcat(output,busdata);
			DEBUG_MSG("%s",output);
		}
		DEBUG_MSG("OUTPUT %s",output);
		send(fd, &output, strlen(output),0);
		close(fd);
		printf("Connection closed...\n");
	}
}

/* Thread function used by Data Supplier thread */
void *data_supplier_thread_fxn(void *args) {
	data_supplier_thread_data_t * data_supplier_thread_data = (data_supplier_thread_data_t *)args;
	
	/* Create and bind the socket.
	 */
	int skt;
	
	DEBUG_MSG("Starting Data Supplier thread at port %d", data_supplier_thread_data->port);
	
	if ((skt = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
		perror("socket");
		exit(1);
	}

	struct sockaddr_in addr;
	addr.sin_family = AF_INET;
	addr.sin_port = htons(data_supplier_thread_data->port);
	addr.sin_addr.s_addr = INADDR_ANY;
	if (bind(skt, (struct sockaddr *) &addr, sizeof(addr)) < 0) {
		perror("bind");
		exit(1);
	}

	if (listen(skt, 1) < 0) {
		perror("listen");
		exit(1);
	}
		
	do_supply(skt, data_supplier_thread_data);
	return 0;
}

/* Thread function used by Data Collector threads */
void *data_collector_thread_fxn(void *args) {

	data_collector_thread_data_t* dc_thread_data = (data_collector_thread_data_t *)args;
	DEBUG_MSG("Starting Data Collector thread %d on port %d",dc_thread_data->thread_id, dc_thread_data->port);

	/* Create and bind the socket */
	int s;
	if ((s = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
		perror("socket");
		exit(1);
	}

	struct sockaddr_in addr;
	addr.sin_family = AF_INET;
	addr.sin_port = htons(dc_thread_data->port);
	addr.sin_addr.s_addr = INADDR_ANY;

	int yes=1;
	if (setsockopt(s,SOL_SOCKET,SO_REUSEADDR,&yes,sizeof(int)) == -1) {
	    perror("Set sockopt");
	    exit(1);
	}

	if (bind(s, (struct sockaddr *) &addr, sizeof(addr)) < 0) {
		perror("bind");
		exit(1);
	}

	do_collect(s, dc_thread_data);
	return 0;
}

/* If called without arguments, listen for connections.  Otherwise make a
 * connection to the specified first argument.
 */
int main(int argc, char *argv[]){
	int i;
	/* Threads to open servers to gather data from DC */
	pthread_t *data_collector_thread_ids;

	/* Thread to open server to supply data to OC */
	pthread_t data_supplier_thread_id;

	/* Data passed to each data collector thread */
	data_collector_thread_data_t **dc_thread_data_array;

	/* Data passed to data supplier thread */
	data_supplier_thread_data_t data_supplier_thread_data;
	
	/* Get and parse arguments */
	get_args(argc, argv);

	init_bus_details();

	int dc_start_port = DFL_DC_PORT;
	int ds_port = DFL_DS_PORT;
	int dc_instance = MAX_DC_SERVER_THREADS;

	/* DC start port must be positive */
	if (prog_args.dc_start_port != 0) {
		if ((dc_start_port = atoi(prog_args.dc_start_port)) <= 0) {
			fprintf(stderr, "%s: port must be positive integer\n",
			        prog_args.name);
			exit(1);
		}
	}
	/*DS port must be positive */
	if (prog_args.ds_port != 0) {
		if ((ds_port = atoi(prog_args.ds_port)) <= 0) {
			fprintf(stderr, "%s: port must be positive integer\n",
			        prog_args.name);
			exit(1);
		}
	}
	/* DC number of instance must be positive */
	if (prog_args.dc_instance != 0) {
		if ((dc_instance = atoi(prog_args.dc_instance)) <= 0) {
			fprintf(stderr, "%s: DC instance must be positive integer\n",
			        prog_args.name);
			exit(1);
		}
	}
	
	data_collector_thread_ids = (pthread_t*) malloc(sizeof(pthread_t)*dc_instance);
	if (NULL == data_collector_thread_ids) {
		fprintf(stderr, "%s: Unable to allocate memory \n", prog_args.name);
		exit(1);
	}
	dc_thread_data_array = (data_collector_thread_data_t **)malloc(sizeof(data_collector_thread_data_t *)*dc_instance);
	if (NULL == dc_thread_data_array ) {
		fprintf(stderr, "%s: Unable to allocate memory \n", prog_args.name);
		exit(1);
	}

	/* Start Data Collector servers */
	for (i=0; i< dc_instance; i++) {
		dc_thread_data_array[i] = (data_collector_thread_data_t*)malloc(data_collector_thread_data_s);
		dc_thread_data_array[i]->cur_index = 0;
		dc_thread_data_array[i]->port = dc_start_port + i;
		dc_thread_data_array[i]->thread_id = i;
		
		DEBUG_MSG("Creating Data Collector Thread: %d",i);
		pthread_create(	&data_collector_thread_ids[i], NULL,
						data_collector_thread_fxn, (void*)dc_thread_data_array[i] );
	}

	DEBUG_MSG("Creating Data Supplier Thread");
	data_supplier_thread_data.data_collector = dc_thread_data_array;
	data_supplier_thread_data.max_dc_srv_thread = dc_instance;
	data_supplier_thread_data.port  = ds_port;
	/* Data Supplier Thread */
	pthread_create(&data_supplier_thread_id, NULL, data_supplier_thread_fxn, (void*)&data_supplier_thread_data);

	/* Wait for Data Collector threads */
	for(i=0; i< dc_instance; i++) {
		//pthread_join(data_collector_thread_ids[i], NULL);
	}
	
	/* Wait for Data Supplier thread */
	pthread_join(data_supplier_thread_id, NULL);

	return (0);
}
