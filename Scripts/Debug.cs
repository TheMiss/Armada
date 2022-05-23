// ReSharper disable RedundantUsingDirective
// ReSharper disable InvalidXmlDocComment
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

#if OVERRIDE_UNITYENGINE_DEBUG
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Internal;
using Object = UnityEngine.Object;

public static class Debug 
{
    /// <summary>
    /// Opens or closes developer console.
    /// </summary>
    public static bool developerConsoleVisible
    {
        get => UnityEngine.Debug.developerConsoleVisible;
        set => UnityEngine.Debug.developerConsoleVisible = value;
    }

    /// <summary>
    ///   <para>In the Core Settings dialog there is a check box called "Development Core".</para>
    /// </summary>
    public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

    /// <summary>
    ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
    /// </summary>
    /// <param name="condition">Condition you expect to be true.</param>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="message">String or object to be converted to string representation for display.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition)
    {
        UnityEngine.Debug.Assert(condition);
    }

    /// <summary>
    ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
    /// </summary>
    /// <param name="condition">Condition you expect to be true.</param>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="message">String or object to be converted to string representation for display.</param>
    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, string message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }
    /// <summary>
    ///   <para>Pauses the editor.</para>
    /// </summary>
    public static void Break()
    {
        UnityEngine.Debug.Break();
    }

    /// <summary>
    ///   <para>Clears errors from the developer console.</para>
    /// </summary>
    public static void ClearDeveloperConsole()
    {
        UnityEngine.Debug.ClearDeveloperConsole();
    }

    /// <summary>
    ///   <para>Draws a line between specified start and end points.</para>
    /// </summary>
    /// <param name="start">Point in world space where the line should start.</param>
    /// <param name="end">Point in world space where the line should end.</param>
    /// <param name="color">Color of the line.</param>
    /// <param name="duration">How long the line should be visible for.</param>
    /// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
    public static void DrawLine(Vector3 start, Vector3 end)
    {
        UnityEngine.Debug.DrawLine(start, end);
    }

    /// <summary>
    ///   <para>Draws a line between specified start and end points.</para>
    /// </summary>
    /// <param name="start">Point in world space where the line should start.</param>
    /// <param name="end">Point in world space where the line should end.</param>
    /// <param name="color">Color of the line.</param>
    /// <param name="duration">How long the line should be visible for.</param>
    /// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        UnityEngine.Debug.DrawLine(start, end, color);
    }

    /// <summary>
    ///   <para>Draws a line between specified start and end points.</para>
    /// </summary>
    /// <param name="start">Point in world space where the line should start.</param>
    /// <param name="end">Point in world space where the line should end.</param>
    /// <param name="color">Color of the line.</param>
    /// <param name="duration">How long the line should be visible for.</param>
    /// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration);
    }

    /// <summary>
    ///   <para>Draws a line between specified start and end points.</para>
    /// </summary>
    /// <param name="start">Point in world space where the line should start.</param>
    /// <param name="end">Point in world space where the line should end.</param>
    /// <param name="color">Color of the line.</param>
    /// <param name="duration">How long the line should be visible for.</param>
    /// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
    public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color,
        [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    /// <summary>
    ///   <para>Draws a line from start to start + dir in world coordinates.</para>
    /// </summary>
    /// <param name="start">Point in world space where the ray should start.</param>
    /// <param name="dir">Direction and length of the ray.</param>
    /// <param name="color">Color of the drawn line.</param>
    /// <param name="duration">How long the line will be visible for (in seconds).</param>
    /// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
    public static void DrawRay(Vector3 start, Vector3 dir)
    {
        UnityEngine.Debug.DrawRay(start, dir);
    }

    /// <summary>
    ///   <para>Draws a line from start to start + dir in world coordinates.</para>
    /// </summary>
    /// <param name="start">Point in world space where the ray should start.</param>
    /// <param name="dir">Direction and length of the ray.</param>
    /// <param name="color">Color of the drawn line.</param>
    /// <param name="duration">How long the line will be visible for (in seconds).</param>
    /// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        UnityEngine.Debug.DrawRay(start, dir, color);
    }

    /// <summary>
    ///   <para>Draws a line from start to start + dir in world coordinates.</para>
    /// </summary>
    /// <param name="start">Point in world space where the ray should start.</param>
    /// <param name="dir">Direction and length of the ray.</param>
    /// <param name="color">Color of the drawn line.</param>
    /// <param name="duration">How long the line will be visible for (in seconds).</param>
    /// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration);
    }

    /// <summary>
    ///   <para>Draws a line from start to start + dir in world coordinates.</para>
    /// </summary>
    /// <param name="start">Point in world space where the ray should start.</param>
    /// <param name="dir">Direction and length of the ray.</param>
    /// <param name="color">Color of the drawn line.</param>
    /// <param name="duration">How long the line will be visible for (in seconds).</param>
    /// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
    public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color,
        [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }

    /// <summary>
    ///   <para>Logs a message to the Unity Console.</para>
    /// </summary>
    /// <param name="message">String or object to be converted to string representation for display.</param>
    /// <param name="context">Object to which the message applies.</param>
    [Conditional("DEBUG")]
    public static void Log(object message, Object context = null)
    {
        UnityEngine.Debug.Log(message, context);
    }

    /// <summary>
    ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
    /// </summary>
    /// <param name="message">String or object to be converted to string representation for display.</param>
    /// <param name="context">Object to which the message applies.</param>
    public static void LogError(object message, Object context = null)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    /// <summary>
    ///   <para>Logs a formatted error message to the Unity console.</para>
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <param name="context">Object to which the message applies.</param>
    public static void LogErrorFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(format, args);
    }

    /// <summary>
    ///   <para>Logs a formatted error message to the Unity console.</para>
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <param name="context">Object to which the message applies.</param>
    public static void LogErrorFormat(Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(context, format, args);
    }

    /// <summary>
    ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
    /// </summary>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="exception">Runtime Exception.</param>
    public static void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    /// <summary>
    ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
    /// </summary>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="exception">Runtime Exception.</param>
    public static void LogException(Exception exception, Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }

    /// <summary>
    ///   <para>Logs a formatted message to the Unity Console.</para>
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="logType">Type of message e.g. warn or error etc.</param>
    /// <param name="logOptions">Option flags to treat the log message special.</param>
    [Conditional("DEBUG")]
    public static void LogFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);
    }

    /// <summary>
    ///   <para>Logs a formatted message to the Unity Console.</para>
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="logType">Type of message e.g. warn or error etc.</param>
    /// <param name="logOptions">Option flags to treat the log message special.</param>
    [Conditional("DEBUG")]
    public static void LogFormat(Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(context, format, args);
    }

    /// <summary>
    ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
    /// </summary>
    /// <param name="message">String or object to be converted to string representation for display.</param>
    /// <param name="context">Object to which the message applies.</param>
    public static void LogWarning(object message, Object context = null)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }

    /// <summary>
    ///   <para>Logs a formatted warning message to the Unity Console.</para>
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <param name="context">Object to which the message applies.</param>
    public static void LogWarningFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(format, args);
    }

    /// <summary>
    ///   <para>Logs a formatted warning message to the Unity Console.</para>
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <param name="context">Object to which the message applies.</param>
    public static void LogWarningFormat(Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(context, format, args);
    }
}

#endif
