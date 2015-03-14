#include <stdio.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <errno.h>
#include <stdlib.h>

#define MAXBUF 1024
#define MAX_LOOP 1024
#define SERVER_PORT 9999

void do_recv(int s){
	int fd, i;
	char buffer[MAXBUF];
	for (i = 0; i < MAX_LOOP; i++) {
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
		/*---Echo back anything sent---*/
		send(fd, buffer, recv(fd, buffer, MAXBUF, 0), 0);
	}
	
	close(fd);
	printf("Connection closed...\n");

}

int main(int argc, char *argv[])
{
	int port = SERVER_PORT;
	
	/* Create and bind the socket.
	 */
	int skt;
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

	do_recv(skt);
	return 0;
}
