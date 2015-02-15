#!/usr/bin/env python
# outputs (timestamp, pktlen) pairs for packets sampled from the union of
# 2-dimensional Gaussian distributions in the (pktsize,datarate) plane.
import random
import sys

MIN_PACKET_SIZE = 30 # our timing header = 30 bytes
MAX_PACKET_SIZE = 1024*1024 # max send size of 1 MB
MIN_DATA_RATE = 100
MAX_DATA_RATE = 100000000 # 100 Mbps

class Distribution:
	def __init__(self, spec):
		fields = spec.split(",")
		self.msize = float(fields[0])
		self.mrate = float(fields[1])
		self.dsize = float(fields[2])
		self.drate = float(fields[3])

def sample(dist, duration):
	samples = []
	currtime = 0.0

	while True:
		# determine length of packet
		def genlen():
			return int(round(random.gauss(dist.msize, dist.dsize)))
		length = genlen()
		while not (MIN_PACKET_SIZE <= length <= MAX_PACKET_SIZE):
			length = genlen()

		# determine data rate
		def genrate():
			return random.gauss(dist.mrate, dist.drate)
		rate = genrate()
		while not (MIN_DATA_RATE <= rate <= MAX_DATA_RATE):
			rate = genrate()

		# convert data rate to delay until next packet
		# bitsperbyte * bytesperpacket / rate
		delay = 8 * length / rate

		# update current time, and return if we've gone past the end
		currtime += delay
		if currtime < duration:
			samples.append((currtime, length - MIN_PACKET_SIZE))
		else:
			return samples

if __name__ == '__main__':
	if len(sys.argv) < 3:
		sys.exit("Usage: %s duration distribution ...\n\t%s"
				% (sys.argv[0], "where distribution = size_mean,rate_mean,size_dev,rate_dev"))
	# duration is in seconds...
	duration = float(sys.argv[1])
	dists = []
	for i in sys.argv[2:]:
		dists.append(Distribution(i))

	events = []
	for d in dists:
		events += sample(d, duration)

	# sort all packets 
	events.sort(cmp=lambda a,b: cmp(a[0], b[0]))

	for i in events:
		(ts, length) = i
		print "%f %d" % (ts, length)
