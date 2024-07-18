
difference to supervised learning:
does not rely on knowing the right action to take (from label created by knowledgable external supervisor)

difference to unsupervised learning:
reinforcement learning is trying to maximize a reward signal instead of trying to find hidden structure.

We therefore consider reinforcement learning to be a third machine learning paradigm, alongside of supervised learning, unsupervised learning, and perhaps other paradigms as well.

the system is not told what actions to take
taken actions influence later states to learn from
no training data set predefined or manual labeling necessary
In interactive problems it is often impractical to obtain examples of desired behavior that are both correct and representative of all the situations in which the agent has to act.
One of the challenges that arise in reinforcement learning, and not in other kinds of learning, is the trade-off between exploration and exploitation.

In a biological system, we might think of rewards as analogous to the experiences of pleasure or pain. <- maybe change the reward over time because at first human child will get pleasure from standing up or walking a few steps but over time pleasure will lessen or change to bigger goals.

Whereas rewards determine the immediate, intrinsic desirability of environmental states, values indicate the long-term desirability of states after taking into account the states that are likely to follow, and the rewards available in those states.

In fact, the most important component of almost all reinforcement learning algorithms we consider is a method for efficiently estimating values. The central role of value estimation is arguably the most important thing we have learned about reinforcement learning over the last few decades.