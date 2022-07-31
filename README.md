# ModernBusinessContinuity

வணக்கம்
---

### POWERFUL SPECIFICATIONS

The main program should start with seyals.

We will `create main.c` from:

```
முதல் செயல்
	சொல் வணக்கம்
```

From here lets generate `create types.c`:
```
எண் க
விகிதம் ப
```

```
செயல் புரிந்துதல்
	எண் கண்
	விகிதம் படி
```

The word செயல் is interchangeable.


Back to `append main.c`
We check if cli was given with args or not.
```
	if(argc > 1) {
		s = server_new(argv[1]);
	} else {
		s = server_new("kingdom.json");
	}
```


```
	server_start(s);
```


```
    ...last
	return EXIT_SUCCESS;
```


```first
#ifdef DEBUG
	printf("Starting kingdom\n");
#endif
```

We include stdlib and server.h
```include
"server.h"
<stdlib.h>
```

This would generate server.c
