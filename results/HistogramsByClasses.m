% 4 RuleBased [0.2 0.4 0.2];
[a_hard,a_medium,a_easy,a_frh,a_frm,a_fre] = getFinalPointsByClass('log7.txt');
% 1 RuleBased 3 Random [1 0 0];
[b_hard,b_medium,b_easy,b_frh,b_frm,b_fre] = getFinalPointsByClass('log8.txt');
% 2 RuleBased 2 Random [0.2 0.5 1];
[c_hard,c_medium,c_easy,c_frh,c_frm,c_fre] = getFinalPointsByClass('log9.txt');
% 1 TrickPlayer 3 RuleBased [1 0.84 0];
[d_hard,d_medium,d_easy,d_frh,d_frm,d_fre] = getFinalPointsByClass('log12.txt');
% 2 Trick 2 RuleBased [1 0.42 0];
[e_hard,e_medium,e_easy,e_frh,e_frm,e_fre] = getFinalPointsByClass('log13.txt');
% 4 Trick
[z_hard,z_medium,z_easy,z_frh,z_frm,z_fre] = getFinalPointsByClass('log14.txt');
% 1 Deep 3 RuleBased UF1 [1 0.11 0.52]
[f_hard,f_medium,f_easy,f_frh,f_frm,f_fre] = getFinalPointsByClass('log17.txt');
% 2 Deep 2 RuleBased UF1 [0.36 0.14 0.9]
[g_hard,g_medium,g_easy,g_frh,g_frm,g_fre] = getFinalPointsByClass('log15.txt');
% 1 Deep 3 RuleBased UF2 [0.15 0.95 0.95]
[h_hard,h_medium,h_easy,h_frh,h_frm,h_fre] = getFinalPointsByClass('log18.txt');
% 2 Deep 2 RuleBased UF2 [0.21 0.87 0.08]
[i_hard,i_medium,i_easy,i_frh,i_frm,i_fre] = getFinalPointsByClass('log16.txt');


A = figure(1);
a1 = histfit(a_hard,60);
a1(2).Color = [0.2 0.4 0.2];
delete(a1(1));
hold on;
b1 = histfit(b_hard,60);
b1(2).Color = [1 0 0];
delete(b1(1));
c1 = histfit(c_hard,60);
c1(2).Color = [0.2 0.5 1];
delete(c1(1));
% g1 = histfit(g_hard,60);
% g1(2).Color = [0.36 0.14 0.9];
% delete(g1(1));
axis([0 120 0 1]);
set(gca,'XTick', [0 20 40 60 80 100 120]);

B = figure(2);
a2 = histfit(a_medium,60);
a2(2).Color = [0.2 0.4 0.2];
delete(a2(1));
hold on;
b2 = histfit(b_medium,60);
b2(2).Color = [1 0 0];
delete(b2(1));
c2 = histfit(c_medium,60);
c2(2).Color = [0.2 0.5 1];
delete(c2(1));
% g2 = histfit(g_medium,60);
% g2(2).Color = [0.36 0.14 0.9];
% delete(g2(1));
axis([0 120 0 40]);
set(gca,'XTick', [0 20 40 60 80 100 120]);

C = figure(3);
a3 = histfit(a_easy,60);
a3(2).Color = [0.2 0.4 0.2];
delete(a3(1));
hold on;
b3 = histfit(b_easy,60);
b3(2).Color = [1 0 0];
delete(b3(1));
c3 = histfit(c_easy,60);
c3(2).Color = [0.2 0.5 1];
delete(c3(1));
% g3 = histfit(g_easy,60);
% g3(2).Color = [0.36 0.14 0.9];
% delete(g3(1));
axis([0 120 0 1]);
set(gca,'XTick', [0 20 40 60 80 100 120]);