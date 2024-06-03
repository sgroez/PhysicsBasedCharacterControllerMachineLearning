### Walker Ragdoll
- Hip (root) DOF: 3
	- Spine DOF: 3
		- Chest DOF: 3
			- Head DOF: 2
			- UpperArmL DOF: 2
				- LowerArmL DOF: 1
					- HandL DOF: 0
			- UpperArmR DOF: 2
				- LowerArmR DOF 1
					- HandR DOF: 0
	- ThighL DOF: 2
		- ShinL DOF: 1
			- FootL DOF: 3
	- ThighR DOF: 2
		- ShinL DOF: 1
			- FootL DOF: 3

Overall DOF: 26 (with root 29)

### OnEpisodeBeginn
```
reset bodyparts
randomize root rotation on y axis to value in range 0..360
randomize target walking speed to range 0.1..maxWalkingSpeed
```

### Observation
```
//following three values represent the same data
currentVelocityNormalized (velGoal-avgVel).magnitude
avgVelocityRelativeToCube
velGoalRelativeToCube

//rotation deltas
rotationDeltaHipsAndCubeForward
rotationDeltaHeadAndCubeForward

//target position
targetPositionRelativeToCube

//Bodypart observations
isTouchingGround
bodypartVelocityRelativeToCube
bodypartAngularVelocityRelativeToCube
positionBetweenBodypartAndHipsRelativeToCube

//if bodypart is not hip or hands
localBodypartRotation
jointStrength normalized (currentStrength / maxJointForceLimit)
```

### Action
```
//for every bodypart except hands and hips
setTargetRotations
setJointStrengths
```

### Reward
```
//every fixed update
matchSpeedReward * lookAtTargetReward
//on touched target
touchedTargetReward //not used
```

### EndEpisode
```
on touched ground
after max of 5000 steps
```