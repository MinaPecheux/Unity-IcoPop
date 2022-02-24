public class TargetBaseManager : TargetManager
{

    public override void TakeHit()
    {
        if (!_canBeHit) return;
        base.TakeHit();
        _Die();
    }

}
