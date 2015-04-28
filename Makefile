CC=mcs
RR=mono
FILES = $(wildcard *.cs)
MAINGAMEFILES = $(filter-out MainTest.cs, $(FILES))
MAINTESTFILES = $(filter-out MainSuecaSolver.cs, $(FILES))
EXEMAINFILE=sueca-solver.exe
EXETESTFILE=test.exe

all: clean compile-game run

compile-game:
	$(CC) -out:$(EXEMAINFILE) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(MAINGAMEFILES)

compile-test:
	$(CC) -out:$(EXETESTFILE) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(MAINTESTFILES)

test:
	time $(RR) $(EXETESTFILE)

run:
	time $(RR) $(EXEMAINFILE)

clean:
	rm -rf *.exe
