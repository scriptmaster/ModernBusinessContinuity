
Create `hive.c`:

```#
"event2.h"
stdlib
#include <stdio.h>
#include <event.h>
#include <evhttp.h>
```

Create `ev2.h`:
```c
void event_cb(int fd, short what, void * ptr);
```

`ev2.c`:
```c
//import stdio
//export
void event_cb(int fd, short what, void * ptr)
{
    printf("%p\n", ptr);
}
```

then in `hive.c`:
```include
#include "ev2.h"
```

```c
int main()
{
    printf("Event hive with worker bees!!\n");
    struct event * ev;
    struct event_base * base;

    base = event_base_new();

    int fd = 1;
    ev = event_new(base, fd, (short) EV_READ, event_cb, NULL);
    (void)ev;

    printf("base: %p\n", base);
}
```

```ignore
These ignore blocks are ignored
```
