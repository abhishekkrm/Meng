#!/usr/bin/env python
# take input from gen.py, and add an offset to the time fields
# this lets us combine several distributions over time
import sys

if __name__ == '__main__':
	if len(sys.argv) != 2:
		sys.exit("Usage: %s time_offset" % sys.argv[0])

	offset = float(sys.argv[1])

	for line in sys.stdin:
		(ts, size) = line.split()
		ts = float(ts)
		size = int(size)
		ts += offset
		print "%f %d" % (ts, size)
