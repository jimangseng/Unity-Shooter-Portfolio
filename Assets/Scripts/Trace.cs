using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Trace
{
    // 시간
    public float Time { get; set; }

    // 궤적의 시작점과 끝점
    public Vector3 From { get; set; }
    public Vector3 To { get; set; }

    // 궤적을 이루는 각각의 힘
    Force horizontal;
    Force vertical;
    Force gravity;

    // 현재 위치, 순간 속도
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }

    // 기본 생성자
    public Trace ()
    {
        Time = 0.0f;

        From = Vector3.zero;
        To = Vector3.zero;
        
        horizontal = new Force();
        vertical = new Force();
        gravity = new Force();

        Position = Vector3.zero;
        Velocity = Vector3.zero;
    }

    // 복사 생성자
    public Trace(Trace _previous)
    {
        Time = _previous.Time;

        From = _previous.From;
        To = _previous.To;

        horizontal = new Force(_previous.horizontal);
        vertical = new Force(_previous.vertical);
        gravity = new Force(_previous.gravity);

        Position = _previous.Position;
        Velocity = _previous.Velocity;
    }

    public void setTrace(Vector3 _from, Vector3 _to)
    {
        From = _from;
        To = _to;

        horizontal.InitialVelocity = Vector3.Normalize(_to - _from) * Vector3.Magnitude(_to - _from) ;
        vertical.InitialVelocity = Vector3.up * 15.0f; // 하드코딩 - 왜 15.0f 인가
    }

    public void update()
    {
        Time += UnityEngine.Time.deltaTime;

        calculate();
    }

    public void calculate()
    {
        horizontal.InstantVelocity = (horizontal.Acceleration * Vector3.right) * Time + horizontal.InitialVelocity;
        horizontal.CurrentPosition = (0.5f) * (horizontal.Acceleration * Vector3.right) * Time * Time + horizontal.InstantVelocity * Time + horizontal.InitialPosition;

        vertical.Acceleration = 0.0f;
        vertical.InstantVelocity = (vertical.Acceleration * Vector3.up) * Time + vertical.InitialVelocity;
        vertical.CurrentPosition = (0.5f) * (vertical.Acceleration * Vector3.up) * Time * Time + vertical.InstantVelocity * Time + vertical.InitialPosition;

        gravity.Acceleration = -9.8f;
        gravity.InitialVelocity = Vector3.zero;
        gravity.InstantVelocity = (gravity.Acceleration * Vector3.up) * Time + gravity.InitialVelocity;
        gravity.CurrentPosition = (0.5f) * (gravity.Acceleration * Vector3.up) * Time * Time + gravity.InstantVelocity * Time + gravity.InitialPosition;

        Position = horizontal.CurrentPosition + vertical.CurrentPosition + gravity.CurrentPosition;
        //Velocity = horizontal.instantVelocity + vertical.instantVelocity + gravity.instantVelocity;

        //Position = horizontal.currentPosition;
        //Velocity = horizontal.instantVelocity;
    }

    public Vector3 GetPositionByTime(float _time)
    {
        Time = _time;

        calculate();

        return Position;
    }

    public void Reset()
    {
        Time = 0.0f;

        From = Vector3.zero;
        To = Vector3.zero;

        //horizontal = new Force();
        //vertical = new Force();
        //gravity = new Force();

        Position = Vector3.zero;
        Velocity = Vector3.zero;


    }
}
