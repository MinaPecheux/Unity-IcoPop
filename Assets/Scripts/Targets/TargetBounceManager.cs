public class TargetBounceManager : TargetManager
{

    private int _lives;

    protected override void Awake()
    {
        base.Awake();
        _lives = 2;
    }

    public override void TakeHit()
    {
        if (!_canBeHit) return;
        base.TakeHit();

        // (elastic collision)
        _rigidbody.velocity = -_rigidbody.velocity;
        // (reduce size)
        transform.localScale *= 0.7f;

        _lives--;
        if (_lives == 0)
            _Die();
    }

}
