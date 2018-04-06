from pathlib import Path
import os.path
import math
import numpy as np
import scipy
from scipy.stats import entropy

def main():
    distributionsFile = Path('../results/kl-divergence/RBORuleBased.txt')
    totalSamples = 0
    HBODist = np.zeros(12)
    HumanDist = np.zeros(12)
    
    if distributionsFile.is_file():
        file = open('../results/kl-divergence/RBORuleBased.txt','r') 
        
        line = file.readline()
        i = 0
        while line:
            totalSamples += 1
            line = line.replace('\n','')
            splitLine = line.split(',')
            HBODist[i] = splitLine[0]
            HumanDist[i] = splitLine[1]
            line = file.readline()
            i += 1
        file.close()

        var = scipy.stats.entropy(HumanDist, HBODist)
        print('KL = ', str(var))
      

    else:
        print('HBORuleBased file not found.')
        return


if __name__ == "__main__":
    main()
