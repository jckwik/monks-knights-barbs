/* Third version
	Implements the behavior tree from the expanded attack slide
	Uses several Prolog tricks, like cut (!), fail, assert, retract
*/
clear :- retractall(barbDist(_)),			% Clear the current distance
	retractall(monDist(_)).
	
setBarbDist(Dist) :- assert(barbDist(Dist)).	% Set the distance
setMonDist(Dist) :- assert(monDist(Dist)).	% Set the distance

root :- fleeBarb.
root :- seekMon.
root :- wanderMon.

fleeBarb :- barbDist,
	write('Barbarian too close, Flee it. '),
	nl,
	closestBarb,
	checkDir,
	move.
	
seekMon :- farFromMon,
	write('Too far from a monastery, seek closest safe one. '),
	nl,
	checkSafety,
	checkDist,
	seekClosest.
	
wanderMon :- write('Wander around your current Monastery').

barbDist :- write('Checking Barbarian Distance'),
	nl,
	barbDist(Dist),
	Dist < 25.
	
closestBarb :- write('Check all barbarians, which is closest?'), nl.

checkDir :- write('Check the direction and position of the closest barbarian.'), nl.

move :- write('Move opposite barbarians direction and position.'), nl.

farFromMon :- write('Checking Monastery Distance'),
	nl,
	monDist(Dist),
	Dist > 40.
	
checkSafety :- write('Check all monasteries, make a list of safe ones.'), nl.

checkDist :- write('Check the distance of all safe monasteries, note closest.'), nl.

seekClosest :- write('Seek the closest, safe monastery'), nl.