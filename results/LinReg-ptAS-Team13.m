log = load('log1.txt');
pi_1 = log(:,2);
pi_3 = log(:,4);
ti_1 = log(:,6);
ti_3 = log(:,8);
Ai_1 = log(:,10);
Ai_3 = log(:,12);
Si_1 = log(:,14);
Si_3 = log(:,16);
pf_13 = 120 - log(:,17);
tbl = table(pi_1, pi_3, ti_1, ti_3, Ai_1, Ai_3, Si_1, Si_3, pf_13,'VariableNames',{'pi_1','pi_3','ti_1','ti_3','Ai_1','Ai_3','Si_1','Si_3','pf_13'});
mdl = fitlm(tbl,'pf_13 ~ pi_1 + pi_3 + ti_1 + ti_3 + Ai_1 + Ai_3 + Si_1 + Si_3')
result = table2array(mdl.Coefficients);
save('LinRegResult2.txt', 'result', '-ascii');
