using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IJobQueue
    {
        void Push(Action job);

    }
    public class JobQueue : IJobQueue
    {
        // 해야할 일들을 가지고 있는 큐.
        // Queue의 실행 방법(누가 실행하는가) 1: 일단 push후, 메인 스레드나 다른 스레드가 실행
        // 2: send처럼 처음 들어온(push하는) 스레드가 실행
        Queue<Action> _jobQueue = new Queue<Action>();
        object _lock = new object();
        bool _flush = false; // 2번 실행 방식의 구현, false이면 아무도 실행하지 않아서 들어오는 스레드가 queue를 비움.

        public void Push(Action job)
        {
            // 락이 걸려있으면 강제로 풀기 위함.
            bool flush = false;
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
                if(_flush == false)
                {
                    // flush = _flush = true;//와 차이가 있을지
                    _flush = true;
                    flush = true;
                }
            }

            if(flush)
            {
                Flush();
            }
        }

        void Flush()
        {
            while(true)
            {
                Action action = Pop();
                if(action == null)
                {
                    return;
                }
                action.Invoke();
            }
        }
        Action Pop()
        {
            lock (_lock)
            {
                if (_jobQueue.Count <= 0)
                {
                    // _flush를 이곳에 하는 이유는 lock이 잡혀있는 구간이기 때문.
                    _flush = false;
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }
    }
}
