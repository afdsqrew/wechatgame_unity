using System;
using System.Linq;
using System.Reflection;
using UniFramework.Machine;
using UnityEngine;
using YooAsset;
using Application = UnityEngine.Application;

internal class FsmStartGame : IStateNode
{
    AOTStartGame startGame = new AOTStartGame();
    void IStateNode.OnCreate(StateMachine machine)
    {
    }
    void IStateNode.OnEnter()
    {
        PatchStepChangedEvent.SendEventMessage("Starting game.");
        startGame.OnEnter();


    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }
}