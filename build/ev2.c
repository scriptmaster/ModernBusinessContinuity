
//import stdio
//export
void event_cb(int fd, short what, void * ptr)
{
    printf("%p\n", ptr);
}
