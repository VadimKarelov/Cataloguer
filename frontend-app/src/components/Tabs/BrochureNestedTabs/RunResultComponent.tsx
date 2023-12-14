import Plot from 'react-plotly.js';
import React, {useEffect} from "react";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";

import {SHOULD_USE_ONLY_DB_DATA} from "../../../constants/EnvironmentVariables";
import { Spin } from 'antd';

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
    const historyPoints = props.brochureStore?.runPoints ?? [];

    /**
     * Источник данных для графика предсказанных значений.
     */
    const predictedPoints = props.brochureStore?.predictedRunPoints ?? [];

    /**
     * Значения X точек.
     */
    const xs = SHOULD_USE_ONLY_DB_DATA ? historyPoints.map(point => point.date) : [2, 3, 4];

    /**
     * Значения Y точек.
     */
    const ys = SHOULD_USE_ONLY_DB_DATA ? historyPoints.map(point => point.income) : [100, 110, 120];

    /**
     * Значения X точек.
     */
    const predictedXs = SHOULD_USE_ONLY_DB_DATA ? predictedPoints.map(point => point.date) : [2, 3, 4];

    /**
     * Значения Y точек.
     */
    const predictedYs = SHOULD_USE_ONLY_DB_DATA ? predictedPoints.map(point => point.income) : [100, 110, 120];

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

    /**
     * Выбранный каталог.
     */
    const brochure = props.brochureStore?.currentBrochure ?? null;

    return (
        <Spin size={"large"} spinning={props.brochureStore?.isLoadingChart}>
                <Plot
                    data={[
                        {
                            x: brochure ? [brochure.date, brochure.date] : ["", ""],
                            y: [0, Math.max(...ys) * 1.10],
                            name: "Выпуск каталога",
                            type: 'scatter',
                            line: {
                                dash: 'dot',
                                width: 2
                            }
                        },
                        {
                            x: xs,
                            y: ys,
                            xaxis: 'date',
                            yaxis: 'income',
                            name: "Доход по истории продаж",
                            type: 'scatter'
                        },
                        {
                            x: predictedXs,
                            y: predictedYs,
                            xaxis: 'date',
                            yaxis: 'income',
                            name: "Предсказанный доход",
                            type: 'scatter'
                        },
                    ]}
                    style={{width: "calc(100vw - 500px)", height: "calc(100vh - 200px)", marginTop: 5}}
                    layout={layout}
                    config={{responsive: true}}
                />
        </Spin>
    );
}));

export default RunResultComponent;