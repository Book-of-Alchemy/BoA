using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : SceneBase
{
    public override SceneType SceneType => SceneType.Loading;

    public override void OnEnter()
    {
        Debug.Log("로딩 씬 진입");
    }

    public override void OnExit()
    {
        Debug.Log("로딩 씬 종료");
    }
} 