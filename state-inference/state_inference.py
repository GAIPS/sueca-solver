from pathlib import Path
import os.path
import math
import numpy as np
from sklearn import linear_model
import re
from sklearn.metrics import precision_recall_fscore_support

def main():
    processedhandsFile = Path('..\sueca-logs\processedPlays.txt')
    totalSamples = 0
    abstractHands = {}
    numPlayFeatures = 0
    numHandFeatures = 0
    featuresName = []
    playClassification = ['1','2','4','5','6','8','9','10','12','13','14','16']
    numClasses = len(playClassification)
    
    if processedhandsFile.is_file():
        file = open('..\sueca-logs\processedPlays.txt','r') 
        firstLine = file.readline()
        splitLine = firstLine.split(',')
        numPlayFeatures = int(splitLine[0])
        numHandFeatures = int(splitLine[1]) + 1
        numSamples = int(splitLine[2])
        X = np.ones((numSamples, numHandFeatures)) #coef of 1 to w0
        y = np.zeros(numSamples)

        secondLine = file.readline()
        splitLine = secondLine.split(',')
        for name in splitLine:
            featuresName.append(name)

        line = file.readline()
        i = 0
        while line:
            totalSamples += 1
            line = line.replace('\n','')
            splitLine = line.split(',')
            classification = splitLine[0]
            handFeatures = splitLine[1:]
            X[i][:-1] = handFeatures
            y[i] = int(classification)
            line = file.readline()
            i += 1
        file.close()

        model = linear_model.SGDClassifier(loss='log')
        borderLine = int(0.3 * totalSamples)
        model.fit(X[:borderLine], y[:borderLine])
        print('Borderline: %i', borderLine)
        print('Coefficients: \n', model.coef_)
        print("Mean squared error: %.2f" % np.mean((model.predict(X) - y) ** 2))
        print('Variance score: %.2f' % model.score(X[(borderLine+1):], y[(borderLine+1):]))
        
        prec, rec, fbeta, supp = precision_recall_fscore_support(y[(borderLine+1):], model.predict(X[(borderLine+1):]), labels=model.classes_)
        print('precision: %.2f\n', prec)
        print('recall: %.2f\n', rec)

        with open('weights.txt', 'wb') as file:
            for line in model.coef_:
                line.tofile(file, sep=' ', format='%.6f')
                file.write(b'\r\n')


    else:
        print('processedPlays file not found.')
        return


if __name__ == "__main__":
    main()
