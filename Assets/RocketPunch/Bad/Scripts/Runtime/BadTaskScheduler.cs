using System.Collections.Generic;
using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadTaskScheduler : MonoBehaviour
    {
        public static BadTaskScheduler instance
        {
            get
            {
                if( _instance == null )
                {
                    var go = new GameObject( "BadTaskScheduler" );
                    _instance = go.AddComponent<BadTaskScheduler>();
                    
                    DontDestroyOnLoad( go );
                }
                return _instance;
            }
        }

        private static BadTaskScheduler _instance;
        
        private Queue<BadAsyncLoadTask> _pendingTasks = new();

        private BadAsyncLoadTask _currentTask;
        
        public void EnqueueTasks( List<BadAsyncLoadTask> tasks )
        {
            foreach( var task in tasks )
            {
                _pendingTasks.Enqueue( task );
            }
        }

        private void Update()
        {
            if( _currentTask != null )
            {
                return;
            }
            
            this.StartNextAsyncTask();
        }

        private void StartNextAsyncTask()
        {
            if( _pendingTasks.Count == 0 )
            {
                return;
            }
            
            _currentTask = _pendingTasks.Dequeue();
            _currentTask.complete += OnTaskCompleted;
            _currentTask.Run();
        }

        private void OnTaskCompleted( BadAsyncLoadTask task )
        {
            _currentTask.complete -= OnTaskCompleted;
            _currentTask = null;
            this.StartNextAsyncTask();
        }
    }
}