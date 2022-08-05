watch:
#	watchexec -e md -w src/ -- watchexec -e cs dotnet run
	watchexec -e cs -w . -- watchexec -e md -w src/ dotnet run

OUT=hive
OBJS=build/hive.o build/ev2.o

CFLAGS2 ?= -Wall -Wextra -Isrc -Isrc/jansson/src -Isrc/http-parser
CFLAGS ?= -Wall -MD
LDFLAGS ?= -levent -pthread

$(OUT): $(OBJS) Makefile
	$(CC) -o $(OUT) $(OBJS) $(LDFLAGS)

%.o: %.c %.h Makefile
	$(CC) -c $(CFLAGS) -o $@ $<

%.o: %.c Makefile
	$(CC) -c $(CFLAGS) -o $@ $<

clean:
	rm -f $(OBJS) $(OUT)
	find build/ -name '*.d' -delete
