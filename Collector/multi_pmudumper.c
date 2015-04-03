#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <pthread.h>
#include <string.h>
#include <math.h>
#include <netinet/in.h>
#include "c37.h"

#define DEBUG_ENABLE

#define MAX_DC_SERVER_THREADS (4)
#define BUSCONFIG_FILENAME       "busdetails.txt"
#define BUSMAP_FILENAME			 "bus_map.txt"
#define DFL_DC_PORT	(3361)
#define MAX_HISTORY (10)
#define DFL_DS_PORT (9999)
#define BUF_SIZE 4096
#define MAX_BUS 100
#define PHASE_ANGLE_THRESHOLD		0.5

const double EPS = 1e-20;

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
	uint32_t	soc; 				/* Time Stamp in c37 terminology */
	float		voltage_amplitude; 	/* Reported to OC */
	float 		voltage_angle;		/* Reported to OC */
} dc_data_entry_t;

/* A structure passed to every data-collector-server-thread */
typedef struct data_collector_thread_data {
	int 	port;		/* Port on which this thread is listening */
	int 	thread_id;	/* Id */
	int		cur_index;	/* Current index in the below array */
	dc_data_entry_t 	received_data[MAX_HISTORY]; /* data from DC */
} data_collector_thread_data_t;

#define data_collector_thread_data_s (sizeof(data_collector_thread_data_t))

/* A structure representing details of a bus */
typedef struct bus_details {
	int 	bus_id; 				/* ID of the bus */
	char*	bus_name;				/* Name of the bus */
	double	bus_base_kilo_voltage;	/* Base kilo voltage of bus */
	int		bus_type;				/* Type of bus */
	int		area_number; 			/* Area number */
	char*	bus_info_serialized;	/* All the above fields in comma separated format */
	int* 	connected_buses;		/* Buses which are connected to this bus */
} bus_details_t;

/* A structure passed to data-supplier thread */
typedef struct data_supplier_thread_data {
	int 			port;				/* Port on which this Data Supplier thread is listening */
	int				num_dc_svr_thread; 	/* Number of Data Collector threads = number of buses */
	bus_details_t*	buses_information;	/* An array of all bus information */
	data_collector_thread_data_t **data_collector;
} data_supplier_thread_data_t;


/* Global stuff gleaned from program arguments.
 */
struct prog_args {
	char *name;
	char *dc_start_port;		/* Starting port for data collector threads */
	char *num_dc_svr_threads;	/* Number of data collector threads */
	char *ds_port;				/* Data supplier port */
} prog_args;

static void usage(){
	fprintf(stderr, "Usage: %s [args]\n", prog_args.name);
	fprintf(stderr, "Optional argument:\n");
	fprintf(stderr, "	-c port: TCP data collector starting port [default = %d]\n", DFL_DC_PORT);
	fprintf(stderr, "	-i total instance: Number of pmudumper instance [default = %d]\n", MAX_DC_SERVER_THREADS);
	fprintf(stderr, "	-s port: TCP data supplier port [default = %d]\n", DFL_DS_PORT);
	exit(1);
}

/* Reads and parses busdetails.txt and busmap.txt and populates the given array of bus details */
static void init_bus_details(bus_details_t* bus_details_arr, int num_buses) {
	FILE *fp = NULL;
	char *line = NULL;
	char *token = NULL;
	size_t len = 0, data_len = 0;
	ssize_t read = 0;
	int cur_bus = 0, connected_bus_index = 0;
	
	/* Parse busdetails.txt */
	fp = fopen(BUSCONFIG_FILENAME, "r");
	while ((read = getline(&line, &len, fp)) != -1) {
		
		/* Strip newline character from end */
		token = strtok(line,"\n");
		
		/* Fill serialized data */
		data_len = strlen(token);
		bus_details_arr[cur_bus].bus_info_serialized = (char*) malloc((data_len+1)*sizeof(char));
		strncpy(bus_details_arr[cur_bus].bus_info_serialized, token, data_len);
		bus_details_arr[cur_bus].bus_info_serialized[data_len] = '\0';
				
		/* Also deserialize it */
		/* get bus id */
		token = strtok(token, ",");
		sscanf(token, "%d", &bus_details_arr[cur_bus].bus_id);
		
		/* get bus name */
		token = strtok(NULL, ",");
		data_len = strlen(token);
		bus_details_arr[cur_bus].bus_name = (char*) malloc((data_len+1)*sizeof(char));
		strncpy(bus_details_arr[cur_bus].bus_name, token, data_len);
		bus_details_arr[cur_bus].bus_name[data_len] = '\0';
		
		/* get base kilo voltage */
		token = strtok(NULL, ",");
		sscanf(token, "%lf", &bus_details_arr[cur_bus].bus_base_kilo_voltage);
		
		/* get bus type */
		token = strtok(NULL, ",");
		sscanf(token, "%d", &bus_details_arr[cur_bus].bus_type);
		
		/* get area number */
		token = strtok(NULL, ",");
		sscanf(token, "%d", &bus_details_arr[cur_bus].area_number);
		
		bus_details_arr[cur_bus].connected_buses = NULL;
		cur_bus++;
		
		/* avoid overflow */
		if(cur_bus >= num_buses) {
			break;
		}
	}
	fclose(fp);

	/* Parse busmap.txt */
	fp = fopen(BUSMAP_FILENAME, "r");
	while ((read = getline(&line, &len, fp)) != -1) {
		
		/* get current bus */
		token = strtok(line, ":");
		sscanf(token, "%d", &cur_bus);
		
		/* get connected buses */
		token = strtok(NULL, ":");
		if(token != NULL) {
			bus_details_arr[cur_bus].connected_buses = (int *) malloc(sizeof(int) * (strlen(token)/2 + 1));
		}
		
		connected_bus_index = 0;
		token = strtok(token, ",");
		while (token!= NULL) {
			sscanf(token, "%d", &bus_details_arr[cur_bus].connected_buses[connected_bus_index]);
			connected_bus_index++;
			token = strtok(NULL, ",");
		}
		bus_details_arr[cur_bus].connected_buses[connected_bus_index] = -1;
	}
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
		//write_c37_packet_readable(stdout, pkt);
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
				if (prog_args.num_dc_svr_threads != 0) {
					fprintf(stderr, "%s: can specify number of DC instance\n",
					                prog_args.name);
					exit(1);
				}
				if ((prog_args.num_dc_svr_threads = optarg) == 0) {
					fprintf(stderr, "%s: -i takes number of DC instance\n",
					        prog_args.num_dc_svr_threads);
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
	int32_t num_dc_threads = data_supplier_thread_data->num_dc_svr_thread;
	data_collector_thread_data_t **dc_thread_data_array = data_supplier_thread_data->data_collector;
	bus_details_t* buses_information = data_supplier_thread_data->buses_information;
	char affected[BUF_SIZE];
	int connected_bus_index = 0, connected_bus = 0;
	float voltage_angle_diff = 0.0f, voltage_amplitude[MAX_BUS], voltage_angle[MAX_BUS];

	for (; ;) {
		printf("Waiting for connection...\n");
		if ((fd = accept(s, 0, 0)) < 0) {
			perror("accept");
			exit(1);
		}
		DEBUG_MSG("Got connection from Operator console...\n");
		/*
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
		*/
		char output[BUF_SIZE*MAX_BUS]; 
		memset(&output,0,BUF_SIZE*MAX_BUS);
		
		char end_char ='_';
		for (id = 0; id < num_dc_threads; id++) {
			if (id == (num_dc_threads-1)){
				end_char=':';
			}
			char busdata[BUF_SIZE];
			memset(&busdata,0,BUF_SIZE);
			
			/* TODO: Lock for synchronizing read and writes in data_collector_thread_data_t */
			read_index 	= dc_thread_data_array[id]->cur_index;
			
			voltage_amplitude[id] = dc_thread_data_array[id]->received_data[read_index].voltage_amplitude;
			voltage_angle[id] = dc_thread_data_array[id]->received_data[read_index].voltage_angle;
			
			sprintf(busdata,"%s,%f,%f%c", buses_information[id].bus_info_serialized,
										  voltage_amplitude[id], voltage_angle[id], end_char);
			
			strcat(output,busdata);
			DEBUG_MSG("%s",busdata);
		}
		
		end_char = ':';
		for (id = 0; id < num_dc_threads; id++) {
			if (buses_information[id].connected_buses != NULL) {
			
				for(connected_bus_index = 0; buses_information[id].connected_buses[connected_bus_index] != -1; ++connected_bus_index) {
					connected_bus = buses_information[id].connected_buses[connected_bus_index];
					
					voltage_angle_diff = fabs(voltage_angle[id] - voltage_angle[connected_bus]);
					
					if(voltage_angle_diff - PHASE_ANGLE_THRESHOLD > EPS) {
						strcpy(affected, "");
						if (end_char == ':') {
							sprintf(affected,"%d,%d",id, connected_bus);
							end_char ='_';
						} else {
							sprintf(affected,"%c%d,%d", end_char, id, connected_bus);
						}
						strcat(output,affected);
					} 
				}
			}
		}
		
        strcat(output,"\r\n");
		DEBUG_MSG("Data being sent to Operator Console: %s",output);
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

	int dc_start_port = DFL_DC_PORT;
	int ds_port = DFL_DS_PORT;
	int num_dc_svr_threads = MAX_DC_SERVER_THREADS;

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
	if (prog_args.num_dc_svr_threads != 0) {
		if ((num_dc_svr_threads = atoi(prog_args.num_dc_svr_threads)) <= 0) {
			fprintf(stderr, "%s: DC instance must be positive integer\n",
			        prog_args.name);
			exit(1);
		}
	}
	
	/* Array to hold thread ids of all data collector threads */
	data_collector_thread_ids = (pthread_t*) malloc(sizeof(pthread_t)*num_dc_svr_threads);
	if (NULL == data_collector_thread_ids) {
		fprintf(stderr, "%s: Unable to allocate memory \n", prog_args.name);
		exit(1);
	}
	
	/* Array to hold all data objects passed to data collector threads */
	dc_thread_data_array = (data_collector_thread_data_t **)malloc(sizeof(data_collector_thread_data_t *)*num_dc_svr_threads);
	if (NULL == dc_thread_data_array ) {
		fprintf(stderr, "%s: Unable to allocate memory \n", prog_args.name);
		exit(1);
	}
	
	/* Start Data Collector servers */
	for (i=0; i< num_dc_svr_threads; i++) {
		dc_thread_data_array[i] = (data_collector_thread_data_t*)malloc(data_collector_thread_data_s);
		dc_thread_data_array[i]->cur_index = 0;
		dc_thread_data_array[i]->port = dc_start_port + i;
		dc_thread_data_array[i]->thread_id = i;
		
		DEBUG_MSG("Creating Data Collector Thread: %d",i);
		pthread_create(	&data_collector_thread_ids[i], NULL,
						data_collector_thread_fxn, (void*)dc_thread_data_array[i] );
	}
	
	/* Fill up bus details */
	bus_details_t* buses_information = (bus_details_t*)malloc(sizeof(bus_details_t) * num_dc_svr_threads);
	init_bus_details(buses_information, num_dc_svr_threads);
	
	/* Start Data Supplier thread */
	DEBUG_MSG("Creating Data Supplier Thread");
	data_supplier_thread_data.data_collector = dc_thread_data_array;
	data_supplier_thread_data.num_dc_svr_thread = num_dc_svr_threads;
	data_supplier_thread_data.port  = ds_port;
	data_supplier_thread_data.buses_information = buses_information;
	/* Data Supplier Thread */
	pthread_create(&data_supplier_thread_id, NULL, data_supplier_thread_fxn, (void*)&data_supplier_thread_data);

	/* Wait for Data Collector threads */
	for(i=0; i< num_dc_svr_threads; i++) {
		pthread_join(data_collector_thread_ids[i], NULL);
	}
	
	/* Wait for Data Supplier thread */
	pthread_join(data_supplier_thread_id, NULL);
	return (0);
}
