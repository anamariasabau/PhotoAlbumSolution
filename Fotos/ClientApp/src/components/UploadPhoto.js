import React, { Component } from 'react';
import { post } from 'axios';


export class UploadPhoto extends Component {
    static displayName = UploadPhoto.name;

    constructor(props) {
        super(props);
        this.state = { selectedFile: '',uploadStatus:''};

    }


    fileSelectedHandler = (e) => {

        this.setState({ selectedFile: e.target.files[0] , uploadStatus:''});
        console.log(this.state.selectedFile);
    }



    fileUploadHandler = async (e) => {
        e.preventDefault()
        console.log(this.state.selectedFile);
        const formData = new FormData();
        formData.append('image', this.state.selectedFile);
        const config = {
            headers: {
                'content-type': 'multipart/form-data',
            },
        };
        post('api/photos', formData, config)
            .then((response) => {
                this.setState({ uploadStatus: `${response.data}` })
            })
            .catch((error) => {
                this.setState({ status: `Upload Failed ${error}` });
            })
    }

render() {
    return (
        <div>
            <h1>Upload Photo</h1>
            <input type="file" onChange={this.fileSelectedHandler} />
            <button className="btn btn-primary" onClick={this.fileUploadHandler}>Upload</button>
            <br/>
            <p aria-live="polite"><strong>{this.state.uploadStatus}</strong></p>
        </div>
    );
}
}
