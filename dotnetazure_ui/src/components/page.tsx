import { useEffect, useState } from 'react'
import axios from 'axios';

const Page = ()=> {

    type ItemType = {
        id:string;
        title:string;
        description:string;
        createdOnUtc: string;
        updatedOnUtc?: string | null;
    }

    enum Role {
        User1 = 1,
        User2 = 2,
        User3 = 3
    }

    const [userRole,setUserRole] = useState<Role|null>(Role.User3);

    const [items, setItems] = useState<Array<ItemType>>([]);
    const [showModal, setShowModal] = useState(false);
    const [selectedItem, setSelectedItem] = useState<ItemType | null>(null);
    const [title, setTitle] = useState<string>("");
    const [description, setDescription] = useState<string>("");

    useEffect(()=>{
        axios.get("https://localhost:44359/api/TaskItems").then(
            val => {setItems(val.data)}
            
        ).catch(
          err => console.log(err)
        )
    },[]);

    const handleUpdate = (item: ItemType) => {
        setSelectedItem(item);
        setShowModal(true);
    };

    const handleDelete = async (id:string) =>{
        await axios.delete(`https://localhost:44359/api/TaskItems/${id}`);
        await axios.get("https://localhost:44359/api/TaskItems").then(
            val => {setItems(val.data)}
            
        ).catch(
          err => console.log(err)
        )
    }

const update = async () => {
    await axios.put(
        `https://localhost:44359/api/TaskItems/${selectedItem?.id}`,
        selectedItem
    );
    setSelectedItem(null);

    setShowModal(false);
    await axios.get("https://localhost:44359/api/TaskItems").then(
            val => {setItems(val.data)}
            
        ).catch(
          err => console.log(err)
        )
    // Reload the table
};

const save = async () => {
    const item = {title:title,description:description};
    await axios.post(
        `https://localhost:44359/api/TaskItems`,
        item
    );
    setTitle("");
    setDescription("");
    setShowModal(false);
    await axios.get("https://localhost:44359/api/TaskItems").then(
            val => {setItems(val.data)}
            
        ).catch(
          err => console.log(err)
        )
    // Reload the table
};

return (
    <>
    <p>Items:- {items.length}</p>
      <button disabled={userRole==Role.User2} onClick={(e)=>setShowModal(true)}>Add</button>

       {items && items.length && <table style={{borderColor:'black',borderWidth:'1px'}}>
                 <thead>
                    <tr>
                        <th>Title</th>
                        <th>Description</th>
                        <th>CreatedOnUtc</th>
                        <th>UpdatedOnUtc</th>
                        <th>Actions</th>
                    </tr>
                 </thead>
                 <tbody>
                    {items.map(x =>
                        <tr key={x.id}>
                            <td>{x.title}</td>
                            <td>{x.description}</td>
                            <td>{new Date(x.createdOnUtc).toLocaleString()}</td>
                            <td>{x.updatedOnUtc ? new Date(x.updatedOnUtc).toLocaleString() : "-"}</td>
                            <td>
                                <button onClick={() => handleUpdate(x)}>
                                    Update
                                </button>
                                <button onClick={() => handleDelete(x.id)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    )}
                 </tbody>
        </table>}
        {showModal && selectedItem && (
                <div className="modal-overlay">
                    <div className="modal">
                        <h2>Update Task</h2>

                        <div className="form-group">
                            <label>Title</label>
                            <input
                                type="text"
                                value={selectedItem.title}
                                onChange={(e) =>
                                    setSelectedItem({
                                        ...selectedItem,
                                        title: e.target.value
                                    })
                                }
                            />
                        </div>

                        <div className="form-group">
                            <label>Description</label>
                            <textarea
                                value={selectedItem.description}
                                onChange={(e) =>
                                    setSelectedItem({
                                        ...selectedItem,
                                        description: e.target.value
                                    })
                                }
                            />
                        </div>

                        <div className="buttons">
                            <button
                                onClick={() => {
                                    console.log(selectedItem);
                                    update();
                                    setShowModal(false);
                                }}
                            >
                                Save
                            </button>

                            <button onClick={() => setShowModal(false)}>
                                Cancel
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {showModal && !selectedItem && (
                <div className="modal-overlay">
                    <div className="modal">
                        <h2>Add Task</h2>

                        <div className="form-group">
                            <label>Title</label>
                            <input
                                type="text"
                                onChange={(e) =>
                                    setTitle(e.target.value)
                                }
                            />
                        </div>

                        <div className="form-group">
                            <label>Description</label>
                            <textarea
                                onChange={(e) =>
                                   setDescription(e.target.value)
                                }
                            />
                        </div>

                        <div className="buttons">
                            <button
                                onClick={() => {                                    
                                    save();
                                    setShowModal(false);
                                }}
                            >
                                Save
                            </button>

                            <button onClick={() => setShowModal(false)}>
                                Cancel
                            </button>
                        </div>
                    </div>
                </div>
            )}
    </>
)
}

export default Page;