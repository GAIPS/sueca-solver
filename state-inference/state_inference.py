import os.path
import math
import numpy as np
import time
from sklearn import linear_model
from sklearn import tree
from sklearn import neural_network
import re
from sklearn.metrics import precision_recall_fscore_support

def main():
    fileName = '../sueca-logs/processedPlays.txt'
    totalSamples = 0
    abstractHands = {}
    numPlayFeatures = 0
    numHandFeatures = 0
    featuresName = []
    playClassification = ['1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','28','29','30']
    numClasses = len(playClassification)
    
    if os.path.isfile(fileName):
        file = open(fileName,'r') 
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
        while totalSamples < numSamples:
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

        start_time = time.time()
        print('time has started\n')


        #### DECISION TREE
        #model = tree.DecisionTreeClassifier()
        #trainPercentage = 0.6
        #borderLine = int(trainPercentage * totalSamples)
        #model.fit(X[:borderLine], y[:borderLine])


        ### NEURAL NETWORK
        #model = neural_network.MLPClassifier(hidden_layer_sizes=1000, learning_rate='adaptive')
        #trainPercentage = 0.3
        #borderLine = int(trainPercentage * numSamples)
        #model.fit(X[:borderLine], y[:borderLine])


        ### STOCHASTIC GRADIENT DESCENT
        model = linear_model.SGDClassifier(loss='log', n_jobs=2, class_weight='balanced')
        trainPercentage = 0.3
        borderLine = int(trainPercentage * totalSamples)
        model.fit(X[:borderLine], y[:borderLine])
        
        
        with open('weights.txt', 'wb') as file:
            for line in model.coef_:
                line.tofile(file, sep=' ', format='%.6f')
                file.write(b'\r\n')

    
        print 'train time (s): ', time.time() - start_time
        print 'Train partition: ', trainPercentage
        #print 'Coefficients: ', model.coef_
        #print "Mean squared error: ", np.mean((model.predict(X) - y) ** 2)
        #print 'Variance score: ', model.score(X[(borderLine+1):], y[(borderLine+1):])
    


        test_count = 0.0
        success = 0.0
        impossible_label = 0.0
        countWrongLabels = np.zeros(len(X[0]))
        for i in range(borderLine + 1, len(X)):
        #for i in range(borderLine + 1, borderLine + 4):
            if model.predict(X[i:i+1]) == y[i]:
                success += 1
            else:
                if y[i] == 6 or y[i] == 12 or y[i] == 18 or y[i] == 24 or y[i] == 30:
                    impossible_label += 1
                    countWrongLabels[int(model.predict(X[i:i+1]))] += 1
                #if (y[i] <= 12 and model.predict(X[i:i+1]) <= 12) or (y[i] <= 18 and y[i] >= 13 and model.predict(X[i:i+1]) <= 18 and model.predict(X[i:i+1]) >= 13) or (y[i] <= 24 and y[i] >= 19 and model.predict(X[i:i+1]) <= 24 and model.predict(X[i:i+1]) >= 19) or (y[i] >= 25 and model.predict(X[i:i+1]) >= 25):
                    #success += 1
                #if (model.predict(X[i:i+1]) == 1 and X[i:i+1][0][8] == 0) or (model.predict(X[i:i+1]) == 2 and X[i:i+1][0][9] == 0) or (model.predict(X[i:i+1]) == 3 and X[i:i+1][0][10] == 0) or (model.predict(X[i:i+1]) == 4 and X[i:i+1][0][11] == 0) or (model.predict(X[i:i+1]) == 5 and X[i:i+1][0][12] == 0) or (model.predict(X[i:i+1]) == 6 and X[i:i+1][0][18] == 0) or (model.predict(X[i:i+1]) == 19 and X[i:i+1][0][8] == 0) or (model.predict(X[i:i+1]) == 20 and X[i:i+1][0][9] == 0) or (model.predict(X[i:i+1]) == 21 and X[i:i+1][0][10] == 0) or (model.predict(X[i:i+1]) == 22 and X[i:i+1][0][11] == 0) or (model.predict(X[i:i+1]) == 23 and X[i:i+1][0][12] == 0) or (model.predict(X[i:i+1]) == 24 and X[i:i+1][0][18] == 0) or (model.predict(X[i:i+1]) == 13 and X[i:i+1][0][1] == 0) or (model.predict(X[i:i+1]) == 14 and X[i:i+1][0][2] == 0) or (model.predict(X[i:i+1]) == 15 and X[i:i+1][0][3] == 0) or (model.predict(X[i:i+1]) == 16 and X[i:i+1][0][4] == 0) or (model.predict(X[i:i+1]) == 17 and X[i:i+1][0][5] == 0) or (model.predict(X[i:i+1]) == 18 and X[i:i+1][0][6] == 0) or (model.predict(X[i:i+1]) == 7 and X[i:i+1][0][13] - X[i:i+1][0][8] == 0) or (model.predict(X[i:i+1]) == 8 and X[i:i+1][0][14] - X[i:i+1][0][9] == 0) or (model.predict(X[i:i+1]) == 9 and X[i:i+1][0][15] - X[i:i+1][0][10] == 0) or (model.predict(X[i:i+1]) == 10 and X[i:i+1][0][16] - X[i:i+1][0][11] == 0) or (model.predict(X[i:i+1]) == 11 and X[i:i+1][0][17] - X[i:i+1][0][12] == 0) or (model.predict(X[i:i+1]) == 19 and X[i:i+1][0][13] - X[i:i+1][0][8] == 0) or (model.predict(X[i:i+1]) == 20 and X[i:i+1][0][14] - X[i:i+1][0][9] == 0) or (model.predict(X[i:i+1]) == 21 and X[i:i+1][0][15] - X[i:i+1][0][10] == 0) or (model.predict(X[i:i+1]) == 22 and X[i:i+1][0][16] - X[i:i+1][0][11] == 0) or (model.predict(X[i:i+1]) == 23 and X[i:i+1][0][17] - X[i:i+1][0][12] == 0):
                    #impossible_label += 1
                #probs = model.predict_proba(X[i:i+1])
                #print probs
            test_count += 1

        print 'accuracy ' + str(success / test_count)
        print 'impossible ' + str(impossible_label / test_count)
        np.set_printoptions(threshold=np.nan)
        print countWrongLabels

        #prec, rec, fbeta, supp = precision_recall_fscore_support(y[(borderLine+1):], model.predict(X[(borderLine+1):]), labels=model.classes_)
        #print 'precision: ', prec
        #rint 'recall: ', rec
        #print 'f1: ', fbeta
        #print 'support: ', supp

        #print 'mean f1 of all labels: ', np.mean(fbeta)
        #print 'train + test time (s): ', time.time() - start_time



    else:
        print('processedPlays file not found.')
        return


if __name__ == "__main__":
    main()
