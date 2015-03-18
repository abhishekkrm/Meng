#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <pthread.h>
#include <string.h>
#include <netinet/in.h>
#include "c37.h"

#define DEBUG_ENABLE
#define MAX_DC_SEVER_THREADS (4)
#define DFL_PORT	3361
#define MAX_HISTORY (10)
#define SERVER_PORT 9999

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

/* Global stuff gleaned from program arguments.
 */
struct prog_args {
	char *name;
	char *port;
} prog_args;

static void usage(){
	fprintf(stderr, "Usage: %s [args]\n", prog_args.name);
	fprintf(stderr, "Optional argument:\n");
	fprintf(stderr, "	-p port: TCP server port [default = %d]\n", DFL_PORT);
	exit(1);
}

static int do_copy(int fd, data_collector_thread_data_t* dc_thread_data){
	FILE *input = fdopen(fd, "r");
	if (input == 0) {
		fprintf(stderr, "%s: fdopen failed\n", prog_args.name);
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
	while ((c = getopt(argc, argv, "p:")) != -1) {
		switch (c) {
			case 'p':
				if (prog_args.port != 0) {
					fprintf(stderr, "%s: can specify only one port\n",
					                prog_args.name);
					exit(1);
				}
				if ((prog_args.port = optarg) == 0) {
					fprintf(stderr, "%s: -p takes a port argument\n",
					        prog_args.port);
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

static void do_supply(int s, data_collector_thread_data_t** dc_thread_data_array)
{
	int fd, id, read_index;
	float data[MAX_DC_SEVER_THREADS * 2];
	int32_t num_instances = MAX_DC_SEVER_THREADS;
	for (; ;) {
		printf("Waiting for connection...\n");
		if ((fd = accept(s, 0, 0)) < 0) {
			perror("accept");
			exit(1);
		}
		printf("Got connection from ooperator console...\n");
		
		for (id = 0; id < MAX_DC_SEVER_THREADS; id++) {
			/* TODO: Lock for synchronizing read and writes in data_collector_thread_data_t */
			read_index 	= dc_thread_data_array[id]->cur_index;
			
			/* Currently sending data as a float array */
			data[id * 2] 		= dc_thread_data_array[id]->received_data[read_index].voltage_amplitude;
			data[id * 2 + 1] 	= dc_thread_data_array[id]->received_data[read_index].voltage_angle;
		}
		send(fd, &num_instances, sizeof(int32_t),0);
		
		send(fd, &data, sizeof(float) * 2 * MAX_DC_SEVER_THREADS,0);
		
		close(fd);
		printf("Connection closed...\n");
	}
}

/* Thread function used by Data Supplier thread */
void *data_supplier_thread_fxn(void *args) {
	data_collector_thread_data_t** dc_thread_data_array = (data_collector_thread_data_t**)args;
	
	int port = SERVER_PORT;
	
	/* Create and bind the socket.
	 */
	int skt;
	
	DEBUG_MSG("%s", "Starting Data Supplier thread");
	
	if ((skt = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
		perror("socket");
		exit(1);
	}

	struct sockaddr_in addr;
	addr.sin_family = AF_INET;
	addr.sin_port = htons(port);
	addr.sin_addr.s_addr = INADDR_ANY;
	if (bind(skt, (struct sockaddr *) &addr, sizeof(addr)) < 0) {
		perror("bind");
		exit(1);
	}

	if (listen(skt, 1) < 0) {
		perror("listen");
		exit(1);
	}
		
	do_supply(skt, dc_thread_data_array);
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
	pthread_t data_collector_thread_ids[MAX_DC_SEVER_THREADS];
	/* Thread to open server to supply data to OC */
	pthread_t data_supplier_thread_id;
	/* Data passed to each data collector thread */
	data_collector_thread_data_t* dc_thread_data_array[MAX_DC_SEVER_THREADS];
	
	
	/* Get and parse arguments */
	get_args(argc, argv);

	int port = DFL_PORT;
	if (prog_args.port != 0) {
		if ((port = atoi(prog_args.port)) <= 0) {
			fprintf(stderr, "%s: port must be positive integer\n",
			        prog_args.name);
			exit(1);
		}
	}
	
	/* Start Data Collector servers */
	for (i=0; i< MAX_DC_SEVER_THREADS; i++) {
		dc_thread_data_array[i] = (data_collector_thread_data_t*)malloc(data_collector_thread_data_s);
		
		dc_thread_data_array[i]->cur_index = 0;
		dc_thread_data_array[i]->port = port + i;
		dc_thread_data_array[i]->thread_id = i;
		
		DEBUG_MSG("Creating Data Collector Thread: %d",i);
		
		pthread_create(	&data_collector_thread_ids[i], NULL, 
						data_collector_thread_fxn, (void*)dc_thread_data_array[i] );
	}

	DEBUG_MSG("Creating Data Supplier Thread");
	/* Data Supplier Thread */
	pthread_create(&data_supplier_thread_id, NULL, data_supplier_thread_fxn, (void*)dc_thread_data_array);

	/* Wait for Data Collector threads */
	for(i=0; i< MAX_DC_SEVER_THREADS; i++) {
		pthread_join(data_collector_thread_ids[i], NULL);
	}
	
	/* Wait for Data Supplier thread */
	pthread_join(data_supplier_thread_id, NULL);

	return (0);
}
