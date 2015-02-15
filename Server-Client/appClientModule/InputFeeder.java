import java.io.BufferedReader;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

public class InputFeeder {

	protected HashMap<Integer,BusDetails> busDetailsMap;
	private String fileName="busdetails.txt";	
	public HashMap<Integer, ArrayList<Integer>> connectivityMap = new HashMap<Integer, ArrayList<Integer>>();
	private static InputFeeder inputFeeder_instance=null;
	
	private InputFeeder()
	{
		setupConnectivityMap();
		printMap(connectivityMap);
	}
	
	/**
	 *
	 * Singleton design pattern is used to get only one instance of InputFeeder	 
	 * @return
	 */
	public static synchronized InputFeeder getInstance()
	{
		if(inputFeeder_instance==null)
		{
				synchronized(InputFeeder.class)
				{
					if(inputFeeder_instance==null)
					{
						inputFeeder_instance=new InputFeeder();
					}
				}
			
		}
		return inputFeeder_instance;
	}
	
	public void feedNewValues() {
		
		refreshInputFile();		
		busDetailsMap=populateBusDetailsMap();
	}

	private HashMap<Integer, BusDetails> populateBusDetailsMap() 
	{
		BufferedReader in=null;		
		BusDetails busDetails=new BusDetails();
		HashMap<Integer,BusDetails> busDetailsMap=new HashMap<Integer,BusDetails>();
			try {
			in = new BufferedReader(new FileReader(new File(fileName)));				
			String line;
			while ((line = in.readLine()) != null) {				
				busDetails=preprocess(line);	
				busDetailsMap.put(busDetails.getBusNo(), busDetails);
			}
			in.close();
			
		} catch(IOException e) {
			System.out.println(" caught a " + e.getClass() + "\n with message: " + e.getMessage());
		}		
		return busDetailsMap;
 }
	
	
	private BusDetails preprocess(String line) 
	{
		String values[]=line.split("_");
		BusDetails busDetails=new BusDetails();
		busDetails.setBusNo(new Integer(values[0]));
		busDetails.setBusName(values[1]);
		busDetails.setBusBaseKiloVoltage(Double.parseDouble(values[2]));
		busDetails.setBusType(values[3]);
		busDetails.setAreaNumber(new Integer(values[4]));
		busDetails.setBusVoltage(Double.parseDouble(values[5]));
		busDetails.setVoltagePhaseAngle(Double.parseDouble(values[6]));	
		if(connectivityMap.get(new Integer(values[0]))!=null)
		{
			System.out.println(" inside connectivityMap"+connectivityMap.get(new Integer(values[0])).size());
			busDetails.setConnectivityList(connectivityMap.get(new Integer(values[0])));
		}
		printBusDetails(busDetails);
		return busDetails;
	}

	private void printBusDetails(BusDetails busDetails)
	{
		System.out.println("Bus No"+busDetails.getBusNo());
		System.out.println("Area Number"+busDetails.getAreaNumber());
		System.out.println("Bus Base Kilo Voltage"+busDetails.getBusBaseKiloVoltage());
		System.out.println("Bus Type"+busDetails.getBusType());
		System.out.println("Bus Voltage"+busDetails.getBusVoltage());
		System.out.println("Voltage Phase Angle"+busDetails.getVoltagePhaseAngle());
		ArrayList<Integer> toPrintList=busDetails.getConnectivityList();
		for(int i:toPrintList)
		{
			System.out.println("Bus Connectivty List"+i);		
		}		
	}

	
	/**
	 * 
	 */
	private void refreshInputFile()
	{
		BufferedReader in=null;
		File readFile=new File("busdetails.txt");
		File writtenFile = new File("busdetails2.txt");
		
		try {
			
			in = new BufferedReader(new FileReader(readFile));
			FileWriter writer = new FileWriter(writtenFile);
			String readLine;
			while ((readLine = in.readLine()) != null) {				
				//String newLine = readLine.substring(0,readLine.lastIndexOf('_'))+"_"+getPhaseAngle();
				String temp = readLine.substring(0,readLine.lastIndexOf('_'));
				String temp1 = temp.substring(0,temp.lastIndexOf('_'));
				String newLine=temp1+"_"+getVoltage()+"_"+getPhaseAngle();
				writer.write(newLine);
				writer.append('\n');
			}			
			in.close();
			writer.close();
			readFile.delete();
			writtenFile.renameTo(readFile);			
			
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
	
	}
	
	/**
	 * 
	 * @return
	 */
	private double getPhaseAngle()
	{
		int min=0;
		int max=90;
		double phaseAngle=min + (Math.random() * ((max - min) + 1));
		DecimalFormat f = new DecimalFormat("##.0000");
		phaseAngle=Double.parseDouble(f.format(phaseAngle)); 
		return phaseAngle;
	}

	private double getVoltage()
	{
		int min=0;
		int max=2;
		double voltage=min + (Math.random() * ((max - min) + 1));
		DecimalFormat f = new DecimalFormat("##.0000");
		voltage=Double.parseDouble(f.format(voltage)); 
		return voltage;
	}

	@Override
	public String toString() {
		return "ReadFile [fileName=" + fileName + "]";
	}



	public void setupConnectivityMap() {
		
		ArrayList<Integer> connectedBusList;
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(3);
		connectedBusList.add(6);
		connectedBusList.add(7);
		AddInHashMap(2,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(2);
		connectedBusList.add(4);
		AddInHashMap(3,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(3);
		connectedBusList.add(5);
		AddInHashMap(4,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(4);
		AddInHashMap(5,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(2);
		connectedBusList.add(17);
		AddInHashMap(6,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(2);
		connectedBusList.add(8);
		connectedBusList.add(12);
		AddInHashMap(7,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(7);
		connectedBusList.add(9);
		connectedBusList.add(25);
		AddInHashMap(8,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(10);
		connectedBusList.add(8);
		connectedBusList.add(13);
		AddInHashMap(9,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(9);
		connectedBusList.add(14);
		AddInHashMap(10,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(7);
		connectedBusList.add(18);
		AddInHashMap(12,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(9);
		connectedBusList.add(14);
		connectedBusList.add(16);
		AddInHashMap(13,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(10);
		connectedBusList.add(13);
		AddInHashMap(14,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(20);
		connectedBusList.add(13);
		AddInHashMap(16,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(6);
		connectedBusList.add(36);
		connectedBusList.add(27);
		connectedBusList.add(22);
		connectedBusList.add(35);
		AddInHashMap(17,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(12);
		connectedBusList.add(22);
		connectedBusList.add(23);
		AddInHashMap(18,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(16);
		connectedBusList.add(26);
		AddInHashMap(20,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(18);
		connectedBusList.add(17);
		AddInHashMap(22,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(18);
		connectedBusList.add(24);
		connectedBusList.add(33);
		connectedBusList.add(31);
		connectedBusList.add(30);
		AddInHashMap(23,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(23);
		connectedBusList.add(25);
		AddInHashMap(24,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(8);
		connectedBusList.add(24);
		connectedBusList.add(26);
		AddInHashMap(25,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(20);
		connectedBusList.add(25);
		AddInHashMap(26,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(17);
		connectedBusList.add(35);
		AddInHashMap(27,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(23);
		AddInHashMap(30,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(23);
		connectedBusList.add(38);
		AddInHashMap(31,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(34);
		connectedBusList.add(23);
		AddInHashMap(33,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(38);
		connectedBusList.add(33);
		AddInHashMap(34,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(17);
		connectedBusList.add(27);
		AddInHashMap(35,connectedBusList);
		
		
		
		connectedBusList = new ArrayList<Integer>();
		connectedBusList.add(34);
		connectedBusList.add(31);
		AddInHashMap(38,connectedBusList);
		
		
	}
	

	//printer method
	public void printMap(HashMap<Integer,ArrayList<Integer>> map) {
		// TODO Auto-generated method stub
		for(Map.Entry<Integer,ArrayList<Integer>> entry :  map.entrySet()){
			System.out.println(entry.getKey() + " : "+entry.getValue());
		}
	}

	
	public void AddInHashMap(Integer busNo, ArrayList<Integer> connectedBusList)	{
		
		connectivityMap.put(busNo,connectedBusList);
	}

	/**
	 * 
	 * @param args
	 */
	public static void main(String args[])
	{
		InputFeeder inputFeeder=new InputFeeder();
		inputFeeder.feedNewValues();
	}
}
