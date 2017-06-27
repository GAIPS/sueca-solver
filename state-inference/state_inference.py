from pathlib import Path
import os.path
import math
import numpy as np
from sklearn import linear_model

def main():
    processedhandsFile = Path('..\sueca-logs\processedPlays.txt')
    numHands = 0
    abstractHands = {}
    numPlayFeatures = 0
    numHandFeatures = 0
    featuresName = []
    playClassification = ['0','2','3','4','10','11']
    
    if processedhandsFile.is_file():
        file = open(processedhandsFile,'r') 
        firstLine = file.readline()
        splitLine = firstLine.split('\t')
        numPlayFeatures = int(splitLine[0])
        numHandFeatures = int(splitLine[1])
        numSamples = int(splitLine[2])
        X = np.ones((numSamples, numHandFeatures )) #coef of 1 to w0
        y = np.zeros(numSamples)

        secondLine = file.readline()
        splitLine = secondLine.split('\t')
        for name in splitLine:
            featuresName.append(name)

        line = file.readline()
        i = 0
        while line:
            numHands += 1
            splitLine = line.split('\t')
            classification = splitLine[0]
            handFeatures = splitLine[1:]
            X[i] = handFeatures
            y[i] = classification
            line = file.readline()
            i += 1

        
        model = linear_model.SGDClassifier(loss='log')
        model.fit(X, y)
        #print('Coefficients: \n', regr.coef_)
        #print("Mean squared error: %.2f" % np.mean((regr.predict(X) - y) ** 2))
        #print('Variance score: %.2f' % regr.score(X, y))


        file.close()
    else:
        print('processedPlays file not found.')
        return


if __name__ == "__main__":
    main()
