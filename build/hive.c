// "event2.h"
#include <stdlib.h>
#include <stdio.h>
#include <event.h>
#include <evhttp.h>
#include "ev2.h"

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
