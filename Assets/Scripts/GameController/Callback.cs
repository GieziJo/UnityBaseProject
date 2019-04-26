public delegate void CallbackMessenger();
public delegate void CallbackMessenger<T>(T arg1);
public delegate void CallbackMessenger<T, U>(T arg1, U arg2);
public delegate void CallbackMessenger<T, U, V>(T arg1, U arg2, V arg3);