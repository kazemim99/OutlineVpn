public class SportQuotaStateMachineDefinition
{

    public enum dsdf
    {
        RejectDirectBossConfirmWait,
        RejectDistrictSupervisorConfirmWait,
        DirectBossConfirmWait,
        DistrictSupervisorConfirmWait,
        RejectInssuranceUnitChiefConfirmWait,
        HR_InsuranceOfficeChiefConfirmWait

    }
    //public static StateMachine GetStateMachineDefinition(bool isBranchAndUrban)
    //{
    //    StateMachine stateMachine = new StateMachine();

    //    List<MessageState> draftEquivalent =
    //       new List<MessageState>
    //       {
    //                MessageState.RejectDirectBossConfirmWait,
    //                MessageState.RejectDistrictSupervisorConfirmWait,
    //                MessageState.RejectInssuranceUnitChiefConfirmWait,
    //       };
    //    StateInfo draftStateInfo = new StateInfo(MessageState.Draft, null, draftEquivalent, new TargetPermissionState(PermissionType.Self));
    //    stateMachine.Add(draftStateInfo);


    //    RejectState directBossRejectState = new RejectState(MessageState.Draft, MessageState.RejectDirectBossConfirmWait);
    //    StateInfo directBossStateInfo = new StateInfo(MessageState.DirectBossConfirmWait, new List<RejectState>() { directBossRejectState }
    //    , null, new TargetPermissionState(PermissionType.PositionPermission));
    //    stateMachine.Add(directBossStateInfo);

    //    if (isBranchAndUrban)// شهرستان - شعبه ای
    //    {
    //        //رییس منطقه
    //        RejectState rejectDistrictSupervisor = new RejectState(MessageState.Draft, MessageState.RejectDistrictSupervisorConfirmWait);
    //        StateInfo districtSupervisorConfirmWait = new StateInfo(MessageState.DistrictSupervisorConfirmWait
    //            , new List<RejectState>() { rejectDistrictSupervisor }, null, new TargetPermissionState(PermissionType.Custom));
    //        stateMachine.Add(districtSupervisorConfirmWait);
    //    }
    //    else
    //    {
    //        //کارشناس بیمه و رفاه
    //        RejectState rejectInssuranceUnitChief = new RejectState(MessageState.Draft, MessageState.RejectInssuranceUnitChiefConfirmWait);
    //        StateInfo insuranceOfficeChiefConfirmWait = new StateInfo(MessageState.HR_InsuranceOfficeChiefConfirmWait
    //            , new List<RejectState>() { rejectInssuranceUnitChief }, null, new TargetPermissionState(PermissionType.Permission));
    //        stateMachine.Add(insuranceOfficeChiefConfirmWait);
    //    }


    //    return stateMachine;
    //}
}