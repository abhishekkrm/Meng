#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <pthread.h>
#include <string.h>
#include <netinet/in.h>
#include "c37.h"

#define DEBUG_ENABLE
#define MAX_DUMPER_INSTANCE (4)
#define DFL_PORT	3361
#define MAX_HISTORY (10)

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

typedef struct pmudumper_conf {
	int port;
	int thread_id;
} pmudumper_conf_t;
#define pmudumper_conf_s (sizeof(pmudumper_conf_t))

typedef struct pmudumper_data {
	int lock;
	uint32_t soc;
	float voltage_amplitude;
	float voltage_angle;
} pmudumper_data_t;

pmudumper_data_t dumper_data[MAX_DUMPER_INSTANCE][MAX_HISTORY];

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

int do_copy(int fd,int id){
	FILE *input = fdopen(fd, "r");
	if (input == 0) {
		fprintf(stderr, "%s: fdopen failed\n", prog_args.name);
		exit(1);
	}
	int cur_idx = 0;
	/* Copy input.
	 */
	while (!feof(input)) {
		/* Read one frame.
		 */
		char buf[FRAME_SIZE];
		int n = fread(buf, FRAME_SIZE, 1, input);
		if (n == 0) {
			break;
		}
		if (n < 0) {
			perror("do_copy: fread");
			exit(1);
		}

		/* Convert the frame.
		 */
		c37_packet *pkt = get_c37_packet(buf);
		DUMP_PACKET(pkt);
		dumper_data[id][cur_idx].voltage_amplitude = pkt->voltage_amplitude;
		dumper_data[id][cur_idx].voltage_angle = pkt->voltage_amplitude;
		dumper_data[id][cur_idx].soc = pkt->soc;
		cur_idx = (cur_idx + 1) % MAX_HISTORY;

		if (pkt == 0) {
			fprintf(stderr, "%s: do_copy: bad packet\n", prog_args.name);
			exit(1);
		}
		if (pkt->framesize != FRAME_SIZE) {
			fprintf(stderr, "%s: do_copy: bad frame size\n", prog_args.name);
			exit(1);
		}

		/* Write the packet to standard output.
		 */
		write_c37_packet_readable(stdout, pkt);
		free(pkt);
	}

	fclose(input);

	return 1;
}

void do_recv(int s, int id){
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
		do_copy(fd, id);
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

void *pmudumper_client_handler() {//void *args) {
	DEBUG_MSG("%s", "DUMPER Client Thread");

	return (0);
}

void *pmudumper_dc_handler(void *args) {

	pmudumper_conf_t * pmudumper_conf = (pmudumper_conf_t *)args;
	DEBUG_MSG("Starting Dumper Thread %d on port %d",pmudumper_conf->thread_id, pmudumper_conf->port);

	/* Create and bind the socket.
	 */
	int s;
	if ((s = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
		perror("socket");
		exit(1);
	}

	struct sockaddr_in addr;
	addr.sin_family = AF_INET;
	addr.sin_port = htons(pmudumper_conf->port);
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

	do_recv(s, pmudumper_conf->thread_id);
	return 0;
}

/* If called without arguments, listen for connections.  Otherwise make a
 * connection to the specified first argument.
 */
int main(int argc, char *argv[]){
	int i;
	pthread_t pmudumper_dc[MAX_DUMPER_INSTANCE];
	pmudumper_conf_t *pmudumper_conf[MAX_DUMPER_INSTANCE];
	pthread_t pmudumper_client;

	int	 pmudumper_dc_cnt=0;

	get_args(argc, argv);

	int port = DFL_PORT;
	if (prog_args.port != 0) {
		if ((port = atoi(prog_args.port)) <= 0) {
			fprintf(stderr, "%s: port must be positive integer\n",
			        prog_args.name);
			exit(1);
		}
	}

	/* DC dumper handler */
	for (i=0; i< MAX_DUMPER_INSTANCE; i++) {
		pmudumper_conf[i] = (pmudumper_conf_t *)malloc(pmudumper_conf_s);
	//	memset(pmudumper_conf[i], 0,pmudumper_conf_s);
		pmudumper_conf[i]->thread_id = i;
		pmudumper_conf[i]->port  = port + i; //can be taken from a file
		DEBUG_MSG("DC dumper %d",i);
		pthread_create(&pmudumper_dc[pmudumper_dc_cnt++], NULL, pmudumper_dc_handler, (void*)pmudumper_conf[i]);
	}

	DEBUG_MSG("Client dumper");
	/* Client Handler */
	pthread_create(&pmudumper_client, NULL, pmudumper_client_handler, (void*)NULL);

	/* Wait for DC dumper Handler */
	for(i=0; i< pmudumper_dc_cnt; i++) {
		pthread_join(pmudumper_dc[i], NULL);
	}
	/* Wait for Client dumper Handler */
	pthread_join(pmudumper_client, NULL);

	return (0);
}
