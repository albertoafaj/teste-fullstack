import React, { useState } from 'react'

export default function CsvUploadPage(){
  const [log, setLog] = useState(null)

  async function handleUpload(e){
    e.preventDefault()
    const file = e.target.file.files[0]
    const fd = new FormData()
    fd.append('file', file)
    const r = await fetch((import.meta.env.VITE_API_URL || 'http://localhost:5000') + '/api/import/csv', {
      method: 'POST',
      body: fd
    })
    const j = await r.json()
    setLog(j)
    }

    function renderRelatorio(log) {
        if (!log) return <p>Aguardando upload...</p>

        return (
            <div>
                <p>
                    <strong>Processados:</strong> {log.processados} |{' '}
                    <strong>Inseridos:</strong> {log.inseridos} |{' '}
                    <strong>Falhas:</strong> {log.erros?.length || 0}
                </p>

                {log.erros?.length > 0 && (
                    <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: 12 }}>
                        <thead>
                            <tr>
                                <th style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>Linha</th>
                                <th style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>Motivo do Erro</th>
                                <th style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>Dados</th>
                            </tr>
                        </thead>
                        <tbody>
                            {log.erros.map((err, idx) => {                                
                                const match = err.match(/^Linha\s+(\d+):\s+(.+?)\s+\(raw='(.+)'\)$/)
                                const linha = match ? match[1] : 'N/A'
                                const motivo = match ? match[2] : err
                                const raw = match ? match[3] : ''
                                return (
                                    <tr key={idx}>
                                        <td style={{ borderBottom: '1px solid #eee' }}>{linha}</td>
                                        <td style={{ borderBottom: '1px solid #eee', color: 'red' }}>{motivo}</td>
                                        <td style={{ borderBottom: '1px solid #eee', fontFamily: 'monospace', fontSize: 12 }}>
                                            {raw}
                                        </td>
                                    </tr>
                                )
                            })}
                        </tbody>
                    </table>
                )}

                {log.erros?.length === 0 && <p style={{ color: 'green' }}>Nenhum erro encontrado!</p>}
            </div>
        )
    }

  return (
    <div>
      <h2>Importar CSV</h2>
      <div className="section">
        <form onSubmit={handleUpload} style={{display:'flex', gap:10, alignItems:'center'}}>
          <input type="file" name="file" accept=".csv" />
          <button type="submit">Enviar</button>
        </form>
      </div>

      <h3 style={{marginTop:16}}>Relatório</h3>
          <div className="section">
              {renderRelatorio(log)}
      </div>
    </div>
  )
}
