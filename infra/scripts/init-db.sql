-- Inicialización de PostgreSQL para Nexo Platform

-- Extensión para UUID
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Extensión para pgvector (embeddings) - opcional, si se usa PgVector
-- CREATE EXTENSION IF NOT EXISTS vector;

-- Tabla de usuarios
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(255),
    role VARCHAR(50) NOT NULL DEFAULT 'user',
    tier VARCHAR(50) NOT NULL DEFAULT 'standard',
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP
);

-- Tabla de documentos
CREATE TABLE IF NOT EXISTS documents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    filename VARCHAR(255) NOT NULL,
    s3_key VARCHAR(500) NOT NULL,
    size_bytes BIGINT NOT NULL,
    mime_type VARCHAR(100),
    status VARCHAR(50) DEFAULT 'pending',
    embedding_version VARCHAR(20) DEFAULT 'v1',
    chunk_count INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_documents_user_id (user_id),
    INDEX idx_documents_status (status)
);

-- Tabla de queries (auditoría)
CREATE TABLE IF NOT EXISTS queries (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE SET NULL,
    question TEXT NOT NULL,
    response TEXT,
    confidence FLOAT,
    latency_ms INTEGER,
    model_version VARCHAR(20),
    cache_hit BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_queries_user_id (user_id),
    INDEX idx_queries_created_at (created_at)
);

-- Tabla de audit logs
CREATE TABLE IF NOT EXISTS audit_logs (
    id BIGSERIAL PRIMARY KEY,
    user_id UUID REFERENCES users(id),
    action VARCHAR(100) NOT NULL,
    entity_type VARCHAR(50),
    entity_id UUID,
    details JSONB,
    ip_address INET,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_audit_user_id (user_id),
    INDEX idx_audit_created_at (created_at)
);

-- Usuario admin por defecto (contraseña: Admin123!)
INSERT INTO users (id, email, password_hash, full_name, role, tier)
VALUES (
    '11111111-1111-1111-1111-111111111111',
    'admin@nexo.com',
    '$2a$11$K8QkZ9xQ9xQ9xQ9xQ9xQ9uQg8Q8Q8Q8Q8Q8Q8Q8Q8Q8Q8Q8Q8Q8',
    'System Administrator',
    'admin',
    'enterprise'
) ON CONFLICT (email) DO NOTHING;

-- Índices adicionales para performance
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_documents_created_at ON documents(created_at DESC);
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_queries_confidence ON queries(confidence) WHERE confidence IS NOT NULL;

-- Función para actualizar updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_documents_updated_at BEFORE UPDATE ON documents FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();