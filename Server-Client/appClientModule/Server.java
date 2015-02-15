import java.io.DataInputStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map.Entry;

public class Server {

	public static String buffer;
	public static ServerSocket listener;	
	public HashMap<Integer,BusDetails> busDetailsMap;
	FeederThread feederThread=null;
	InputFeeder inputFeeder=InputFeeder.getInstance();
	
	public Server()
	{
		if ((feederThread == null)) {
			   feederThread = new FeederThread(true);
	           Thread thread = new Thread(feederThread);
	           thread.start();
	        }			
	}
	
	public String calculatePhaseDifference()
	{
		String busWithHighVoltageStr = "";
		String busDetailsStr = "";
		for (Entry<Integer, BusDetails> busDetailsMapEntry: busDetailsMap.entrySet()) {
			int busNo = busDetailsMapEntry.getKey();
			BusDetails busDetails = busDetailsMapEntry.getValue();
			busDetailsStr = busDetailsStr.concat(Integer.toString(busNo) + "," + busDetails.getBusName() + "," + 
							Double.toString(busDetails.getBusBaseKiloVoltage()) + "," + 
							busDetails.getBusType() + "," +
							busDetails.getAreaNumber() + "," + 
							busDetails.getBusVoltage() + "," +
							busDetails.getVoltagePhaseAngle() + "_");
			ArrayList<Integer> connectivityList=busDetails.getConnectivityList();
			for (Integer connectedBusNo : connectivityList ){
				if (Math.abs(busDetailsMap.get(connectedBusNo).getVoltagePhaseAngle() - busDetailsMap.get(busNo).getVoltagePhaseAngle()) > Constants.PHASE_ANGLE_THRESOLD)	{
					int busNumber = busDetailsMap.get(connectedBusNo).getBusNo();
					System.out.println("BUS DETAILS:::"+Integer.toString(busNo) + ","  + Integer.toString(busNumber));
					busWithHighVoltageStr = busWithHighVoltageStr.concat(Integer.toString(busNo) + ","  + Integer.toString(busNumber) + "_");
				}
			}
		}
		busDetailsStr = busDetailsStr.substring(0,busDetailsStr.length() - 1);
		busWithHighVoltageStr = busWithHighVoltageStr.substring(0,busWithHighVoltageStr.length() - 1);
		busDetailsStr = busDetailsStr.concat(":" + busWithHighVoltageStr);	//bus details + affected list
		System.out.println("BEFORE SENDING TO CLIENT ::::" + busDetailsStr);
		return busDetailsStr;
	}

	public String getAffectedList()
	{
		busDetailsMap=inputFeeder.busDetailsMap;
		System.out.println("inside busDetailsMap"+busDetailsMap.size());
		String s=calculatePhaseDifference();	
		return s;
	}
	
	public String getBusDetails(Integer busNo)
	{
		busDetailsMap=inputFeeder.busDetailsMap;
		String busDetails = "";
		busDetails = busDetails.concat(Integer.toString(busNo) + "_" + busDetailsMap.get(busNo).getBusName() + "_" + 
						  Double.toString(busDetailsMap.get(busNo).getBusBaseKiloVoltage()) + "_" + 
						  busDetailsMap.get(busNo).getBusType() + "_" +
						  busDetailsMap.get(busNo).getAreaNumber() + "_" + 
						  Double.toString(busDetailsMap.get(busNo).getBusVoltage()) + "_" +
						  Double.toString(busDetailsMap.get(busNo).getVoltagePhaseAngle()));
		return busDetails;
	}
	
	public void process()
	{
		ServerSocket serverSocket=null;
		String outputToServer=null;
		try {					
			serverSocket = new ServerSocket(Constants.CONNECT_PORT);
			System.out.println("In server before while loop");
			while(true) {
		           Socket socket = serverSocket.accept();
		           System.out.println("New connection accepted " + socket.getInetAddress() + ":" + socket.getPort());
		           byte[] receivedBytes = new byte[Constants.MAX_BUFFER_SIZE];
		           DataInputStream is = new DataInputStream(socket.getInputStream());
		           PrintWriter out = new PrintWriter(socket.getOutputStream(), true);
		           is.read(receivedBytes, 0, receivedBytes.length);
		           String dataFromClient = new String(receivedBytes, 0, receivedBytes.length);
		           System.out.println("Server received: " + dataFromClient);	         
		           String[] dataFromClientArr=dataFromClient.split(Constants.DELIMITER);
		           String operationType=dataFromClientArr[0];
		           	switch(operationType.trim()) {		    	
			    	case "AFFECTEDLIST":   		
			    		outputToServer = readAffectedList();
			    		break;	
			    	case Constants.BUS_DETAILS:   		
			    		outputToServer = readBusDetails(dataFromClient);
			    		break;
					}
					out.println(outputToServer);			
					out.flush();
			}
		}	
		catch (SocketException e) {
			if(serverSocket!=null)
				try {
					serverSocket.close();
				} catch (IOException e1) {
					e1.printStackTrace();
				}
			e.printStackTrace();
		} catch (IOException e) {
			if(serverSocket!=null)
				try {
					serverSocket.close();
				} catch (IOException e1) {
					e1.printStackTrace();
				}
			e.printStackTrace();
		}

}
	
	
private String readAffectedList() {		
		String returnString=getAffectedList();		
		return returnString;
}
	
	
private String readBusDetails(String dataFromClient) {
	String[] dataFromClientArr=dataFromClient.split(Constants.DELIMITER);
	Integer busNo=new Integer(dataFromClientArr[1].trim());
	String returnString=getBusDetails(busNo);		
	return returnString;
}

	
public static void main(String args[])
{
	Server server=new Server();
	server.process();
}
	
}