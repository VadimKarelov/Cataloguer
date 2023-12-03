import Plot from 'react-plotly.js';
import React, {useEffect} from "react";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import {SHOULD_USE_ONLY_DB_DATA} from "../../../constants/Routes";

/**
 * Свойства компонента для построения графика.
 */
interface RunResultComponentProps extends BaseStoreInjector {
}

/**
 * Компонент для построения графика для расчёта.
 */
const RunResultComponent: React.FC<RunResultComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Источник данных для графика.
     */
    const dataSource = props.brochureStore?.runPoints ?? [];

    /**
     * Значения X точек.
     */
    const xs = SHOULD_USE_ONLY_DB_DATA ? dataSource.map(point => point.date) : [2, 3, 4];

    /**
     * Значения Y точек.
     */
    const ys = SHOULD_USE_ONLY_DB_DATA ? dataSource.map(point => point.income) : [100, 110, 120];

    /**
     * Вызывает запрос на получение точек графика.
     */
    useEffect(() => props.brochureStore?.updateRunChartPoints(), [props.brochureStore?.currentBrochure?.id]);

    /**
     * Конфигурация макета.
     */
    const layout = {
        autosize: true,
        title: 'Продажи',
        showlegend: true,
        xaxis: {
            title: {
                text: 'Дата продажи',
                // font: {
                //     family: 'Courier New, monospace',
                //     size: 18,
                //     color: '#7f7f7f'
                // }
            },
        },
        yaxis: {
            title: {
                text: 'Доход',
                // font: {
                //     family: 'Courier New, monospace',
                //     size: 18,
                //     color: '#7f7f7f'
                // }
            }
        },
    };

    return (
        <div style={{width: "calc(100vw - 500px)", height: "calc(100vh - 200px)"}}>
            <Plot
                data={[
                    {
                        x: xs,
                        y: ys,
                        xaxis: 'date',
                        yaxis: 'income',
                        name: "Потенциальный доход",
                        type: 'scatter'
                    },
                ]}
                style={{width: "calc(100vw - 500px)", height: "calc(100vh - 200px)"}}
                layout={layout}
                config={{responsive: true}}
            />
        </div>
    );
}));

export default RunResultComponent;