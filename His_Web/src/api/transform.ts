// Normalize API response keys to camelCase and unwrap common envelopes
// - Converts PascalCase/snake_case keys to camelCase
// - Maps known fields: PublishTime -> publishTime, IsPublished -> isPublished

type AnyObj = Record<string, any>

const toCamel = (key: string) => {
  // snake_case
  if (key.includes('_')) {
    return key
      .toLowerCase()
      .split('_')
      .map((s, i) => (i === 0 ? s : s.charAt(0).toUpperCase() + s.slice(1)))
      .join('')
  }
  // PascalCase -> camelCase
  if (/^[A-Z][A-Za-z0-9]*$/.test(key)) {
    return key.charAt(0).toLowerCase() + key.slice(1)
  }
  return key
}

const mapKnownField = (key: string) => {
  switch (key) {
    case 'PublishTime':
      return 'publishTime'
    case 'IsPublished':
    case 'IsPublish':
      return 'isPublished'
    default:
      return key
  }
}

export function normalizeKeys<T>(input: T): T {
  if (Array.isArray(input)) {
    return input.map((item) => normalizeKeys(item)) as unknown as T
  }
  if (input && typeof input === 'object') {
    const obj = input as AnyObj
    const out: AnyObj = {}
    for (const [k, v] of Object.entries(obj)) {
      const mapped = mapKnownField(k)
      const camel = toCamel(mapped)
      out[camel] = normalizeKeys(v)
    }
    return out as T
  }
  return input
}

// Unwrap common envelope structures and then normalize keys
export function unwrapAndNormalize<T>(data: any): T {
  // common envelopes { data: ... } or { result: ... } or { items: ... }
  let payload = data
  if (payload && typeof payload === 'object') {
    if ('data' in payload) payload = payload.data
    else if ('result' in payload) payload = payload.result
    else if ('items' in payload) payload = payload.items
  }
  return normalizeKeys<T>(payload)
}