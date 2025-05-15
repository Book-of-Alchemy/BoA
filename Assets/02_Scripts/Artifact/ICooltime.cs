
public interface ICooltime
{
    int lefttime { get; } // 남은 턴
    int coolTime { get; set; } // 쿨타임 시간
    int availableTime { get; set; } // 동작 가능한 시간 ( 이시간이 글로벌타임과 쿨타임 시간을 더한값 보다 작아진다면 동작가능)
}
