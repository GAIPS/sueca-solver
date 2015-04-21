CC=mcs
RR=mono
FILES = $(wildcard *.cs)
EXEFILE=sueca-solver.exe
N=10

all: clean compile run

compile:
	$(CC) -out:$(EXEFILE) -optimize+ $(FILES)

run:
	time $(RR) $(EXEFILE) $(N)

clean:
	rm -rf *.exe