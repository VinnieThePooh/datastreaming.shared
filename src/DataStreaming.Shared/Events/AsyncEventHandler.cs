namespace DataStreaming.Events;

public delegate Task AsyncEventHandler<in T>(object obj, T eventArgs);