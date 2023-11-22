import Plot from 'react-plotly.js';
import React from "react";

/**
 * Свойства компонента для построения графика.
 */
interface RunResultComponentProps {
}

/**
 * Компонент для построения графика для расчёта.
 */
const RunResultComponent: React.FC<RunResultComponentProps> = (props) => {
    return (
        <div style={{width: "calc(100vw - 500px)", height: "calc(100vh - 200px)"}}>
            <Plot
                data={[
                    {
                        x: [2, 3, 4],
                        y: [100, 110, 120],
                        xaxis: 'x',
                        yaxis: 'y',
                        type: 'scatter'
                    },
                ]}
                style={{width: "calc(100vw - 500px)", height: "calc(100vh - 200px)"}}
                layout={{autosize: true, title: 'Продажи'}}
                config={{responsive: true}}
            />
        </div>
    );
};

export default RunResultComponent;