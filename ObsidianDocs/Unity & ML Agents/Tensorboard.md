<table>
<tr><th>Name</th><th>Description</th></tr>
<tr><td>Cumulative Reward</td><td>Average Reward Agent got during an Episode</td></tr>
<tr><td>Episode Length</td><td>Amount of actions taken by the Agent during one Episode (max is max_steps / decision_period)</td></tr>
<tr><td>Policy Loss</td><td>The amount the policy is changing</td></tr>
<tr><td>Value Loss</td><td>Average value assigned to all states (this value goes up first when learning and then stabilises / comes down when the values get adjusted)</td></tr>
<tr><td>Beta</td><td>Exploitation Expiration Value (higher value equals more exploration)</td>
<tr><td>Entropy</td><td>Visualises how sure the agent is of its actions (lower means more confident)</td></tr></tr>
<tr><td>Epsilon</td><td>Value set in config and decreases linearly (like set in the config)</td>
<tr><td>Extrinsic Reward</td><td>Like cumulative reward but with extrinsic (curiosity) rewards</td></tr></tr>
<tr><td>Extrinsic Value Estimate</td><td></td></tr>
<tr><td>Learning rate</td><td>Starts with value from config and decreases linearly (like set in config)</td></tr>
</table>