## Keywords to research:
- curse of dimensionality
- dimensionality reduction
- latent spaces
- Hypernetwork https://arxiv.org/pdf/1609.09106.pdf
- Unity ML Agents goal signal
- Unity ML Agents Vector Sensor
- mixture of experts

## Unity ML Agents Best Practices
https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Learning-Environment-Design-Agents.md
- start simple and only add complexity as needed
### Observations
- For the best results when training, you should normalize the components of your feature vector to the range [-1, +1] or [0, 1] both in the observation as well as when reading the action from the ML Model (ppo does this automatically).
	using formula: normalizedValue = (currentValue - minValue)/(maxValue - minValue)
	Normalizing Vectors
	apply formula to all components separately (don't use Vector3.normalize property / function)

	Normalizing Rotations
	Vector3 normalized = rotation.eulerAngles / 180.0f - Vector3.one;  // [-1,1]
	Vector3 normalized = rotation.eulerAngles / 360.0f;  // [0,1]
- Positional information of relevant GameObjects should be encoded in relative coordinates wherever possible. This is often relative to the agent position.
- Stacking refers to repeating observations from previous steps as part of a larger observation.
	This is a simple way to give an Agent limited "memory" without the complexity of adding a recurrent neural network (RNN).
### Rewards
- The reward assigned between each decision should be in the range [-1,1]. Values outside this range can lead to unstable training.
- Positive rewards are often more helpful to shaping the desired behavior of an agent than negative rewards. Excessive negative rewards can result in the agent failing to learn any meaningful behavior.
- reward results rather than actions you think will lead to the desired results
- For locomotion tasks, a small positive reward (+0.1) for forward velocity is typically used.
- If you want the agent to finish a task quickly, it is often helpful to provide a small penalty every step (-0.05) that the agent does not complete the task. In this case completion of the task should also coincide with the end of the episode by calling `EndEpisode()` on the agent when it has accomplished its goal.
- The`reward` value is reset to zero when the agent receives a new decision.
- Rewards are not used during inference by an Agent using a trained model and is also not used during imitation learning.