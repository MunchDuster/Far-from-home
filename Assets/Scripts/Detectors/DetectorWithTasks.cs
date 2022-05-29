using UnityEngine;
using UnityEngine.Events;


public class DetectorWithTasks : MonoBehaviour
{
    [System.Serializable]
    public class DetectionWithCriteria
    {
        public int id;
        public UnityEvent OnDetected;

		private bool hasFired;
		public void Fire(bool firesOnce)
        {
           if(!firesOnce)
           {
				ActuallyFire();
			}
           else
           {
                if(!hasFired)
                {
				    ActuallyFire();
			    }
           }
        }
        
        private void ActuallyFire()
        {
            if(OnDetected != null) OnDetected.Invoke();
			hasFired = true;
		} 
    }
    
    public Task[] tasks;
	public bool firesOnce;

	public DetectionWithCriteria[] detections;

	// On Trigger Enter is called when the collider of another GameObject begins colliding with the collider of this GameObject (while this collider is trigger)
	private void OnTriggerEnter(Collider otherCollider)
	{
		if (otherCollider.gameObject.tag == "Player")
		{
            Trigger();
		}
	}
    public void Trigger()
    {
        int id = GetIdByTasks();
        DetectionWithCriteria detection = GetDetectionbyId(id);
            
        if(detection != null)
        {
			detection.Fire(firesOnce);
		}
    }
    
    public void CompleteTask(int index)
    {
        tasks[index].SetCompleted(true);
    }
    
    private int GetIdByTasks()
    {
        //returns something like: 0100, if there are 4 tasks and the second one is complete
        int id = 0;
        for(int i = 0;i < tasks.Length; i++)
        {
            int power = (int)Mathf.Pow(10, i);
            int val = tasks[i].completed ? 1 : 0; // 1 if true, 0 if false
            id += power * val;
        }
        return id;
    }
    private DetectionWithCriteria GetDetectionbyId(int id)
    {
        for(int i = 0;i < detections.Length; i++)
        {
            if(detections[i].id == id)
            {
                return detections[i];
            }
        }
        return null;
    }
}

