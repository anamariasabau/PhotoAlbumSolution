import React, { Component } from 'react';

export class FetchPhotos extends Component {
    static displayName = FetchPhotos.name;

  constructor (props) {
    super(props);
    this.state = { photos: [], loading: true };

    fetch('api/PhotoAlbum/Photos')
      .then(response => response.json())
      .then(data => {
        this.setState({ photos: data, loading: false });
      });
  }

  static renderPhotos (photos) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th></th>
            <th>Caption</th>
            <th>Photo</th>
          </tr>
        </thead>
        <tbody>
          {photos.map(photo =>
              <tr key={photo.Caption}>
              <td>{photo.Thumbnail}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render () {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
        : FetchPhotos.renderPhotos(this.state.photos);

    return (
      <div>
        <h1>Photo display</h1>
        {contents}
      </div>
    );
  }
}
