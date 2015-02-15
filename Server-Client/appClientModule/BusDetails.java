import java.util.ArrayList;

/**
 * 
 */

/**
 * @author RITIKA
 *
 */
public class BusDetails {

	private Integer busNo,areaNumber;
	private double busBaseKiloVoltage,busVoltage,voltagePhaseAngle;
	private String busName,busType;
	private ArrayList<Integer> connectivityList =new ArrayList<Integer>();
	
	public Integer getBusNo() {
		return busNo;
	}

	public void setBusNo(Integer busNo) {
		this.busNo = busNo;
	}

	public double getVoltagePhaseAngle() {
		return voltagePhaseAngle;
	}

	public void setVoltagePhaseAngle(double voltagePhaseAngle) {
		this.voltagePhaseAngle = voltagePhaseAngle;
	}

	public String getBusName() {
		return busName;
	}

	public void setBusName(String busName) {
		this.busName = busName;
	}

	public Integer getAreaNumber() {
		return areaNumber;
	}

	public void setAreaNumber(Integer areaNumber) {
		this.areaNumber = areaNumber;
	}

	public String getBusType() {
		return busType;
	}

	public void setBusType(String busType) {
		this.busType = busType;
	}	

	public double getBusBaseKiloVoltage() {
		return busBaseKiloVoltage;
	}

	public void setBusBaseKiloVoltage(double busBaseKiloVoltage) {
		this.busBaseKiloVoltage = busBaseKiloVoltage;
	}

	public double getBusVoltage() {
		return busVoltage;
	}

	public void setBusVoltage(double busVoltage) {
		this.busVoltage = busVoltage;
	}

	
	public ArrayList<Integer> getConnectivityList() {
		return connectivityList;
	}

	public void setConnectivityList(ArrayList<Integer> connectivityList) {
		this.connectivityList = connectivityList;
	}

	public BusDetails(int busNo, double busBaseKiloVoltage, double busVoltage,
			double voltagePhaseAngle, String busName, Integer areaNumber,
			String busType) {
		super();
		this.busNo = busNo;
		this.busBaseKiloVoltage = busBaseKiloVoltage;
		this.busVoltage = busVoltage;
		this.voltagePhaseAngle = voltagePhaseAngle;
		this.busName = busName;
		this.areaNumber = areaNumber;
		this.busType = busType;
	}

	public BusDetails() {
		// TODO Auto-generated constructor stub
	}
	
	

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub

	}

}
