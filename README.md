# JobEngine

JobEngine is a **lightweight, Hangfire-inspired background job processing engine for .NET**.  
It provides a simple and extensible framework for scheduling, dispatching, and executing background jobs  
with a focus on **reliability**, **graceful shutdown**, and **clean architecture**.

This project demonstrates advanced .NET concepts such as:
- Multi-threaded background services
- Cancellation token orchestration
- Job dispatching and worker lifecycle management
- Low-level synchronization primitives (`ManualResetEventSlim`, `CountdownEvent`)
- Extensible design for custom storage and middleware
