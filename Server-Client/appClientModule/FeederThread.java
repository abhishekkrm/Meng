

/**
 *
 * @author Anil Kumar Puvvadi
 *
 */
public class FeederThread implements Runnable{
	
	protected volatile boolean application_active = false;  

	
	public FeederThread(boolean application_active)
	{
		this.application_active=application_active;
	}

	@Override
	public void run() {
		
			try {
					while(application_active)
					{
						InputFeeder inputFeeder=InputFeeder.getInstance();
						inputFeeder.feedNewValues();						
						Thread.sleep(Constants.FEEDER_THREAD_SLEEP_TIME_IN_MSEC);
						if(application_active==false)
						{
							this.wait();
						}
					}
								
				} catch (InterruptedException e) {
					e.printStackTrace();
				}
		}
}
