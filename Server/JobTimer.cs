using System.Diagnostics.CodeAnalysis;
using Core;

namespace Server
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int ExecTick;    // 얼마 후 실행할 것인가.
        public Action Action;   // 어떤 것을 실행할 것인가.
        public int CompareTo([AllowNull] JobTimerElem other)
        {
            return ExecTick - other.ExecTick;
        }
    }
    public class JobTimer
    {
        PriorityQueue<JobTimerElem> _priorityQueue = new PriorityQueue<JobTimerElem> ();
        object _lock = new object ();

        public static JobTimer Instance { get; } = new JobTimer ();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElem job;
            job.Action = action;
            job.ExecTick = System.Environment.TickCount + tickAfter;

            lock(_lock)
            {
                _priorityQueue.Push(job);
            }
        }

        public void Flush()
        {
            while(true)
            {
                int now = System.Environment.TickCount;

                JobTimerElem job;

                lock(_lock)
                {
                    // 큐에 실행 할 것이 있을때만 실행되도록.
                    if(_priorityQueue.Count == 0)
                    {
                        break;
                    }

                    job = _priorityQueue.Peek();
                    // 실행 타이밍인지 체크
                    if(job.ExecTick > now)
                    {
                        break;
                    }

                    // 앞에서 다 체크했으므로 이부분 부터는 실행해야 할 타이밍.
                    // 큐에서 제거하고 아래에서 지정한 액션 델리게이트를 호출.
                    _priorityQueue.Pop();
                }
                job.Action.Invoke();
            }
        }
    }
}
