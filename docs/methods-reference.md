# Methods reference

The base class provides basic CRUD like methods already implemented  encapsulating all the low level DynamoDB requests / response handling. 

The version with an additional parameter ```TKey pkId``` which is intended for the ```many``` part in a ```one to many``` relationship.

The version without ```TKey pkId``` parameter is intended for single entities or the ```one``` part in a ```one to many``` relationship.

## Single item retrieving operations:

* ```Task<TEntity> GetItemAsync(TKey id)``` 

* ```Task<TEntity> GetItemAsync(TKey pkId, TKey skId)```

## Multiple items retrieving operations:

* ```Task<IList<TEntity>> GetAllItemsAsync()```

* ```Task<IList<TEntity>> GetItemsByParentIdAsync(TKey pkId)```

* ```Task<int> CountAsync()```

## Single item add, update, delete:

* ```Task AddItemAsync(TEntity item)```
* ```Task AddItemAsync(TKey pkId, TEntity item)```
  
* ```Task UpdateItemAsync(TEntity item)```
* ```Task UpdateItemAsync(TKey pkId, TEntity item)```

* ```Task DeleteItemAsync(TKey id)```
* ```Task DeleteItemAsync(TKey pkId, TKey skId)```

## Batch insert and delete: 
* ```Task BatchAddItemAsync(IEnumerable<TEntity> items)```
* ```Task BatchAddItemAsync(TKey pkId, IEnumerable<TEntity> items)```

* ```Task BatchDeleteItemsAsync(IEnumerable<TEntity> items)```
* ```Task BatchDeleteItemsAsync(TKey pkId, IEnumerable<TEntity> items)```