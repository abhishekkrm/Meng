#Mike Ryan, LiveObjects TextGenerator output parsing script


import time
import sys, getopt
import os, glob

class OFiles:
    flist = {} #file names -> bytes dictionary
    folder = []
    
    def __init__(self,f=None):
        self.folder = f 
        
    
    def addfile(self,path):
        print "processing file: " + path
        fhandle = open(path,"rb")
        bin = fhandle.read()
        fhandle.close()
        #pref, name = os.path.split(path)
        self.flist[path] = bin

    def scandirs(self,path):
        for next in os.listdir(path):
            dir = os.path.join(path,next)
            if os.path.isdir(dir):
                print 'found a directory: ' + dir
                self.scandirs(dir)
            else:
                self.addfile(dir)   
            
    def loadFiles(self):
        path = self.folder
        self.scandirs(path)

    
    

class OutputParser:
    flist = None #file
    s = False #sender to process
    r = False #receiver to process
    
    flines = [] #array of arrays of lines (which themselves are an array of split strings!)
    fident = {} #dictionary of Node Names -> flines file index
    
    #dictionary of dictionary of latencies
    #first key is receiver, second key is sender
    laten = {}

    def __init__(self,filez=None,sender=False,receiver=False):
        self.flist = filez
        if sender:
            self.s = sender
            print "SENDER TO PROCESS SET"
        if receiver:
            self.r = receiver
            print "RECEIVER TO PROCESS SET"
            
        
    def run(self):
    
        #first, put all the lines from all the files in the global array
        #fcount is the total number of files (nodes), indices 0 to fcount-1 in the flines array.
        fcount = 0
        for fname in self.flist.flist:
            print fname
            newa = []
            self.flines.append(newa)
            #print self.flist.flist[fname]
            lines = self.flist.flist[fname].split("\n")
            for line in lines:
                if len(line) > 22 and line[19] == "m" and line[20] == "s" and line [21] == "g":
                    split = line.rstrip().split(" ") #rstrip out the /r...
                    self.flines[fcount].append(split)  
            fcount += 1
        
       
        count = 0
        while count < fcount:
            print self.flines[count][0][3]
            self.fident [ self.flines[count][0][3] ] = count
            count += 1    
        print self.fident
        
        
        for file in self.flines:
            rident = file[0][3]
            self.laten[ rident ] = {}
            
            
            packets = {} #keep track by sident of the current packet number, to detect order failures
            for line in file:
                sident = line[2]
                
                #first packet from a sender to this receiver
                if sident not in self.laten[rident]:
                    if line[4] != "0":
                        print "WARNING: Packet 0 Missing from " + sident + " to " + rident                        
                    packets[sident] = int(line[4]) #start off the packet order counter
                    self.laten[rident][sident] = []
                    #put the latency into the newly created array
                    self.laten[rident][sident].append( float(line[6]) - float(line[5]))
                    continue
                    
                
                currPacket = int(line[4])
                if currPacket != packets[sident] + 1:
                    print "WARNING: Have packet " + repr(currPacket) + " after " + repr(packets[sident]) + " from " + sident + " to " + rident
                    
                #put the latencies in the appropriate array 
                stime = float(line[5])
                rtime = float(line[6])
                self.laten[rident][sident].append( rtime - stime )
                packets[sident] = currPacket
        
        
        #search through all files for the sender and receiver we're looking for.
        if self.s and self.r:
            print "Looking for sender: " + self.s + "   and receiver: " + self.r
            
            latens = self.laten[self.r][self.s]
            print latens
            avg = sum(latens) / len(latens)
            print "Average latency: " + repr(avg) + "\n\n"
            
        elif self.s:
            print "Looking for sender: " + self.s
            print "Looking through all receivers for data..."
            for r in self.laten:
                print "Now on Receiver: " + r
                latens = self.laten[r][self.s]
                print latens
                avg = sum(latens) / len(latens)
                print "Average latency: " + repr(avg) + "\n\n"
                
            
        elif self.r:
            print "Looking at receiver: " + self.r
            print "Looking at all senders for data..."
            for s in self.laten[self.r]:
                print "Now on Sender: " + s
                latens = self.laten[self.r][s]
                print latens
                avg = sum(latens) / len(latens)
                print "Average latency: " + repr(avg) + "\n\n"
        
    
    
        
        
        
                        
        
#Global main is just this:    
def main():

    # parse command line options
    try:
        opts, args = getopt.getopt(sys.argv[1:], ["h"], ["help"])
    except getopt.error, msg:
        print msg
        print "for help use --help"
        sys.exit(2)
    # process options
    for o, a in opts:
        if o in ("-h", "--help"):
            print "NO HELP FOR YOU!"
            print (o,a)
            sys.exit(0)  
    
    folder = False
    sender = False
    receiver = False
	
    # process arguments
    for arg in args:
        if arg.startswith("f:"):
            folder = arg[2:]
            print "FOLDER TO PROCESS:"+folder
        elif arg.startswith("folder:"):
            folder = arg[7:]
            print "FOLDER TO PROCESS:"+folder
        elif arg.startswith("s:"):
            sender = arg[2:]
            print "SENDER TO PROCESS:"+sender 
        elif arg.startswith("sender:"):
            sender = arg[7:]
            print "SENDER TO PROCESS:"+sender 
        elif arg.startswith("r:"):
            receiver = arg[2:]
            print "RECEIVER TO PROCESS:"+receiver
        elif arg.startswith("receiver:"):
            receiver = arg[9:]
            print "RECEIVER TO PROCESS:"+receiver

            
    if folder:
        f = OFiles(folder)
        f.loadFiles()
        c = OutputParser(f,sender,receiver)

        c.run()
    else:
        print "You Must Specify a Folder with the experimental output files!!!"
		
		
if __name__ == "__main__":
    sys.exit(main())     


