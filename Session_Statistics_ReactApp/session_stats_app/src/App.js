import useSWR from 'swr'
import './App.css';
import { Visualisations } from './Visualisations';

// wrap native fetch() for SWR
const fetcher = (...args) => fetch(...args).then(res => res.json())

function App() {
  const { data: statData, error } = useSWR('https://australia-southeast1-infiniracer.cloudfunctions.net/ReadAllAttentionSession', fetcher, { refreshInterval: 5000 });

  return (
    <div className="App">
      <body className="App-header">
        <a href="https://play.google.com/store/apps/details?id=com.SES3AT1.InfiniRacer" target="_blank" rel="noreferrer">
          <img src="./finalBrakes.png" className="App-logo" alt="logo" />
        </a>
        <h2>InfiniRacer Feedback Realtime Statistics <br/>({statData?.sessions?.length || 0} Sessions)</h2>
        {error && <p>Error fetching data</p>}
        {!statData && <p>Loading attention feedback data...</p>}
        {statData && <Visualisations sessions={statData.sessions}/>}
      </body>
    </div>
  );
}

export default App;
