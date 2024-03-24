﻿using backend.Domain.Cores.SubTaskAggregate;

namespace backend.Services
{
    public interface ISubTaskService
    {
        public enum State
        {
            NotDone = 1,
            SuccessbygivingReason = 2,
            SuccesswithoutReason = 3,
            FailuretoStatetheReason = 4,
            FailurWithoutReason = 5
        }
        IEnumerable<SubTask> GetAll();
        public  SubTask EditeSubTask(SubTask subTask);
        public void DelSubTask( Guid id);
        public int ChangeStatus(State state);
        SubTask GetById(Guid id);
    }
}