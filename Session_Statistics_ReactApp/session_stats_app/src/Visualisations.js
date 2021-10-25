import "./Visualisations.css";
import { VictoryChart, VictoryLine, VictoryPie, VictoryAxis, VictoryBoxPlot, VictoryLabel, VictoryGroup, VictoryBar, VictoryLegend } from "victory";
import { useState, useEffect } from "react";

export function Visualisations(props) {
    const [isProcessing, setisProcessing] = useState(true)
    const [processedData, setprocessedData] = useState({})

    useEffect(() => {
        const {sessions} = props;
        // process pie chart for difficulties
        let difficultyPieData = [
            {x: "1", y: 0},
            {x: "2", y: 0},
            {x: "3", y: 0},
            {x: "4", y: 0},
            {x: "5", y: 0},
        ];

        let playtimeAttentionDeltaData = [];
        let scoreAttentionDeltaData = [];
        let scoreDistributionData = [
            {x: 1, y: []},
            {x: 2, y: []},
            {x: 3, y: []},
            {x: 4, y: []},
            {x: 5, y: []},
        ];

        let attentionBarGroupData = [
            [], // Bar1 (Before)
            [], // Bar2 (After)
        ]

        let scorePlaytimeRelationData = [];

        sessions.forEach((element, index) => {
            difficultyPieData[element.difficulty-1].y += 1;
            playtimeAttentionDeltaData.push({
                x: element.playTime,
                y: element.attentionAfter - element.attentionBefore,
            })

            scoreAttentionDeltaData.push({
                x: element.score,
                y: element.attentionAfter - element.attentionBefore,
            })

            scoreDistributionData[element.difficulty-1].y.push(element.score);

            scorePlaytimeRelationData.push({
                x: element.playTime,
                y: element.score,
            })
        });

        function getRandomInt(min, max) {
            // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/random
            min = Math.ceil(min);
            max = Math.floor(max);
            return Math.floor(Math.random() * (max - min) + min); //The maximum is exclusive and the minimum is inclusive
        }

        // Use 5 random samples for bar group graph
        for (let i = 0; i < 5; i++) {
            const rand = getRandomInt(0, sessions.length);
            attentionBarGroupData[0].push({x: i + 1, y: sessions[rand].attentionBefore});
            attentionBarGroupData[1].push({x: i + 1, y: sessions[rand].attentionAfter});
        }

        setprocessedData(
            {
                difficultyPieData,
                playtimeAttentionDeltaData,
                scoreAttentionDeltaData,
                scoreDistributionData,
                attentionBarGroupData,
                scorePlaytimeRelationData,
            }
        );

        setisProcessing(false);
    }, [props])
    
    if (isProcessing) {
        return (
            <p>Processing data...</p>
        )
    } else {
        return (
            <>
            <div className="chartRow">
                <div className="chartItem">
                    <p>Difficulty Distribution</p>
                    <p className="description">Game difficulty distribution, (easiest)1-5(hardest)</p>
                    <VictoryPie
                        data={processedData.difficultyPieData}
                        colorScale={["tomato", "orange", "gold", "cyan", "navy" ]}
                        
                        innerRadius={50}
                        animate={{
                            duration: 2000
                        }}
                    />
                </div>
                <div className="chartItem">
                    <p>Playtime/Attention Delta</p>
                    <p className="description">Relation between playtime (seconds) and attention improvement (delta).</p>
                    <VictoryChart
                        domainPadding={30}
                    >
                        <VictoryLine
                            style={{
                                data: {
                                    stroke: "#c43a31",
                                    strokeWidth: 3
                                },
                            }}
                            data={processedData.playtimeAttentionDeltaData}
                            animate={{
                                duration: 2000,
                                onLoad: { duration: 1000 }
                            }}
                        />
                        <VictoryAxis
                            label="Playtime (seconds)"
                            style={{
                                axisLabel: {
                                    padding: 30
                                }
                            }}
                        />
                        <VictoryAxis dependentAxis
                            label="Attention Rating Delta"
                            style={{
                                axisLabel: {
                                    padding: 30
                                }
                            }}
                        />
                    </VictoryChart>
                </div>
                <div className="chartItem">
                    <p>Score/Attention Delta</p>
                    <p className="description">Relation between score and attention improvement (delta).</p>
                    <VictoryChart
                        domainPadding={30}
                    >
                        <VictoryLine
                            style={{
                                data: {
                                    stroke: "#8080c0",
                                    strokeWidth: 3
                                },
                            }}
                            data={processedData.scoreAttentionDeltaData}
                            animate={{
                                duration: 2000,
                                onLoad: { duration: 1000 }
                            }}
                        />
                        <VictoryAxis
                            label="Score"
                            style={{
                                axisLabel: {
                                    padding: 30
                                }
                            }}
                        />
                        <VictoryAxis dependentAxis
                            label="Attention Rating Delta"
                            style={{
                                axisLabel: {
                                    padding: 30
                                }
                            }}
                        />
                    </VictoryChart>
                </div>
            </div>
            <div className="chartRow">
                <div className="chartItem">
                    <p>Score Distribution</p>
                    <p className="description">Score distribution for each difficulty</p>
                    <VictoryChart domainPadding={20} padding={{ left: 90, bottom: 60, right: 20 }}>
                        <VictoryBoxPlot
                            animate={{
                                duration: 2000,
                                onLoad: { duration: 1000 }
                            }}
                            width={30}
                            boxWidth={20}
                            data={processedData.scoreDistributionData}
                        />
                        <VictoryAxis
                            label="Difficulty 1-5"
                            style={{
                                axisLabel: {
                                    padding: 30
                                }
                            }}
                        />
                        <VictoryAxis dependentAxis
                            label="Score"
                            axisLabelComponent={<VictoryLabel dy={-35} />}
                        />
                    </VictoryChart>
                </div>
                <div className="chartItem">
                    <p>Attention Deltas</p>
                    <p className="description">Visualisation of 5 random sessions, attention rating before and after gameplay.</p>
                    <VictoryChart>
                        <VictoryLegend x={50} y={10}
                            title=""
                            centerTitle
                            orientation="horizontal"
                            gutter={20}
                            style={{ title: {fontSize: 20 } }}
                            data={[
                            { name: "Before", symbol: { fill: "orange" } },
                            { name: "After", symbol: { fill: "teal" } },
                            ]}
                        />
                        <VictoryGroup offset={20}
                            colorScale={"qualitative"}
                            animate={{
                                duration: 1000,
                                onLoad: { duration: 500 }
                            }}
                        >
                            <VictoryBar
                                style={{
                                    data: { fill: "orange" }
                                }}
                                data={processedData.attentionBarGroupData[0]}
                            />
                            <VictoryBar
                                style={{
                                    data: { fill: "teal" }
                                }}
                                data={processedData.attentionBarGroupData[1]}
                            />
                        </VictoryGroup>
                        <VictoryAxis
                            label="Sample"
                            style={{
                                axisLabel: {
                                    padding: 30
                                }
                            }}
                        />
                        <VictoryAxis dependentAxis
                            label="Attention Rating"
                            axisLabelComponent={<VictoryLabel />}
                        />
                    </VictoryChart>
                </div>
                <div className="chartItem">
                    <p>Score/Playtime Relation</p>
                    <p className="description">Relation between score and playtime.</p>
                    <VictoryChart
                        domainPadding={0}
                        padding={{ left: 100, bottom: 60, right: 40 }}
                    >
                        <VictoryLine
                            style={{
                                data: {
                                    stroke: "#8080c0",
                                    strokeWidth: 3
                                },
                            }}
                            data={processedData.scorePlaytimeRelationData}
                            animate={{
                                duration: 2000,
                                onLoad: { duration: 1000 }
                            }}
                        />
                        <VictoryAxis
                            label="Play Time (seconds)"
                            style={{
                                axisLabel: {
                                    padding: 30
                                }
                            }}
                        />
                        <VictoryAxis dependentAxis
                            label="Score"
                            style={{
                                axisLabel: {
                                    padding: 60
                                }
                            }}
                        />
                    </VictoryChart>
                </div>
            </div>
            <a href="https://github.com/ajahiri" target="_blank" rel="noreferrer" className="nameText">Made by: Arian Jahiri (13348469)</a>
            </>
        )
    }
}