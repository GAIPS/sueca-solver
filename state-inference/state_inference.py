from pathlib import Path
import os.path

def main():
    processedhandsFile = Path('..\sueca-logs\processedHands.txt')
    numHands = 0
    abstractHands = {}
    
    if processedhandsFile.is_file():
        file = open(processedhandsFile,'r') 

        for line in file:
            numHands += 1
            if line in abstractHands:
                abstractHands[line] += 1
            else:
                abstractHands[line] = 1

        file.close() 
        print('abstractHands: ' + str(len(abstractHands.keys())))
        print('numHands: ' + str(numHands))


    else:
        print('ProcessedHands file not found.')
        return

    doSomething(abstractHands)

def doSomething(hands):
    for key, value in hands:
        print(key)


if __name__ == "__main__":
    main()
